using Sha;
using SHA256App;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace SHA256_APP
{
    public partial class Form1 : Form
    {
        SHA256 sha256 = new SHA256();
        private string[] inputFilePaths = Array.Empty<string>(); // Tablica z nazwami wybranych plików

        private int threadCount = 1; //domyœlna wartoœæ na razie


        // Tablica wartoœci odpowiadaj¹cych pozycjom TrackBar
        private readonly int[] threadValues = { 1, 2, 4, 8, 16, 32, 64 };
        private int defultThreads;
        public Form1()
        {
            InitializeComponent();

            // Pobranie liczby logicznych procesorów
            defultThreads = Environment.ProcessorCount;

            // Inicjalizacja TrackBar
            trackBar1.Minimum = 0; // Indeks w tablicy threadValues
            trackBar1.Maximum = threadValues.Length - 1; // Ostatni indeks

            // Znalezienie najbli¿szego indeksu dla defultThreads
            int defaultIndex = FindClosestThreadIndex(defultThreads);

            // Ustawienie wartoœci TrackBar na znaleziony indeks
            trackBar1.Value = defaultIndex;

            // Wyœwietlenie wartoœci na starcie
            labelTrackBarValue.Text = threadValues[trackBar1.Value].ToString();

        }

        // Funkcja do znalezienia indeksu najbli¿szej wartoœci w threadValues
        private int FindClosestThreadIndex(int threadCount)
        {
            int closestIndex = 0;
            int minDifference = int.MaxValue;

            for (int i = 0; i < threadValues.Length; i++)
            {
                int difference = Math.Abs(threadValues[i] - threadCount);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Tworzymy okno dialogowe
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Ustawienia dialogu
                openFileDialog.Title = "Select Input Files";
                openFileDialog.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt"; // Filtry plików
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Domyœlna lokalizacja
                openFileDialog.Multiselect = true; // Pozwala na wybór wielu plików

                // Sprawdzenie, czy u¿ytkownik wybra³ pliki
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Pobranie œcie¿ek do wybranych plików
                    inputFilePaths = openFileDialog.FileNames;

                    // Wyœwietlenie wszystkich wybranych plików (dla debugowania lub informacji)
                    string fileList = string.Join("\n", inputFilePaths);
                    MessageBox.Show($"Selected files:\n{fileList}", "Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            threadCount = threadValues[trackBar1.Value];  // Ustawienie odpowiedniej liczby w¹tków
            labelTrackBarValue.Text = threadCount.ToString();  // Wyœwietlenie wartoœci na etykiecie
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (inputFilePaths == null || inputFilePaths.Length == 0)
            {
                MessageBox.Show("Please select at least one input file.", "Input File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Please select a library (ASM or x64).", "Library Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Upewnij siê, ¿e folder wyjœciowy istnieje
            string outputDirectory = "C:/Wyniki";
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string selectedLibrary = radioButton1.Checked ? "asm" : "cs";
            int selectedThreads = threadValues[trackBar1.Value];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var fileGroups = SplitFilesIntoGroups(inputFilePaths, selectedThreads);
            ConcurrentDictionary<string, string> results = new ConcurrentDictionary<string, string>();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = inputFilePaths.Length;
            progressBar1.Value = 0;

            List<Task> tasks = new List<Task>();
            object lockObj = new object(); // Obiekt do synchronizacji zapisu plików

            foreach (var group in fileGroups)
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (string filePath in group)
                    {
                        try
                        {
                            string content = File.ReadAllText(filePath);
                            string hash = sha256.ComputeHash(content, selectedLibrary);
                            results.TryAdd(filePath, hash);

                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            string extension = Path.GetExtension(filePath);

                            // Dodaj unikalny identyfikator do nazwy pliku
                            string uniqueFileName = $"{fileName}_{Guid.NewGuid():N}X{extension}";
                            string outputFilePath = Path.Combine(outputDirectory, uniqueFileName);

                            // Synchronizowany zapis pliku
                            lock (lockObj)
                            {
                                File.WriteAllText(outputFilePath, hash);
                            }

                            this.Invoke(() =>
                            {
                                progressBar1.Value = Math.Min(progressBar1.Value + 1, progressBar1.Maximum);
                                progressBar1.Refresh();
                            });
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(() =>
                            {
                                MessageBox.Show($"Error processing file {filePath}: {ex.Message}",
                                              "Processing Error",
                                              MessageBoxButtons.OK,
                                              MessageBoxIcon.Error);
                            });
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            double elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

            try
            {
                if (progressBar1 != null)
                {
                    progressBar1.Value = progressBar1.Maximum;
                }

                if (labelElapsedTime != null)
                {
                    labelElapsedTime.Text = $"Elapsed Time: {elapsedMs} ms";
                    labelElapsedTime.Refresh();
                }
                else
                {
                    // Jeœli label nie istnieje, poka¿ czas w MessageBox
                    MessageBox.Show($"Process completed.\nElapsed time: {elapsedMs} ms",
                                  "Process Complete",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating UI: {ex.Message}",
                               "UI Update Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning);
            }

            // Poka¿ podsumowanie
            MessageBox.Show($"Processed {results.Count} out of {inputFilePaths.Length} files.\n" +
                           $"Elapsed time: {elapsedMs} ms",
                           "Process Completed",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
        }


        private List<List<string>> SplitFilesIntoGroups(string[] filePaths, int numberOfGroups)
        {
            // Nie tworzymy wiêcej grup ni¿ mamy plików
            numberOfGroups = Math.Min(numberOfGroups, filePaths.Length);

            List<List<string>> groups = new List<List<string>>();

            // Inicjalizacja grup
            for (int i = 0; i < numberOfGroups; i++)
            {
                groups.Add(new List<string>());
            }

            // Obliczamy bazow¹ liczbê plików na grupê
            int filesPerGroup = filePaths.Length / numberOfGroups;
            // Obliczamy ile grup dostanie dodatkowy plik
            int remainingFiles = filePaths.Length % numberOfGroups;

            int currentFileIndex = 0;

            // Rozdzielamy pliki do grup
            for (int i = 0; i < numberOfGroups; i++)
            {
                // Okreœlamy liczbê plików dla aktualnej grupy
                int filesForThisGroup = filesPerGroup + (i < remainingFiles ? 1 : 0);

                // Dodajemy pliki do aktualnej grupy
                for (int j = 0; j < filesForThisGroup; j++)
                {
                    groups[i].Add(filePaths[currentFileIndex]);
                    currentFileIndex++;
                }
            }

            return groups;
        }





        private readonly List<(string path, int expectedCount)> testFolders = new List<(string path, int expectedCount)>
{
    (@"C:\TestFiles\Set30k", 30000),
    (@"C:\TestFiles\Set50k", 50000),
    (@"C:\TestFiles\Set70k", 70000)
};

        private async void button3_Click(object sender, EventArgs e)
        {
            // Results StringBuilder
            StringBuilder results = new StringBuilder();
            results.AppendLine("SHA256 Performance Test Results");
            results.AppendLine("=============================");
            results.AppendLine($"Test Date: {DateTime.Now}\n");

            // Dla ka¿dego folderu testowego
            for (int folderIndex = 0; folderIndex < testFolders.Count; folderIndex++)
            {
                var (folderPath, expectedCount) = testFolders[folderIndex];

                if (!Directory.Exists(folderPath))
                {
                    MessageBox.Show($"Error: Directory not found: {folderPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] testFiles = Directory.GetFiles(folderPath);

                results.AppendLine($"\nTest Set {folderIndex + 1}");
                results.AppendLine($"Folder: {folderPath}");
                results.AppendLine($"Number of files: {testFiles.Length} (Expected: {expectedCount})");
                results.AppendLine("----------------------------------------");

                // Dla ka¿dej biblioteki
                foreach (string library in new[] { "cs", "asm" })
                {
                    results.AppendLine($"\nLibrary: {library.ToUpper()}");

                    // Tabela wyników dla tego zestawu testowego
                    results.AppendLine("\nThreads\tIteration 1\tIteration 2\tIteration 3\tIteration 4\tIteration 5\tAverage");
                    results.AppendLine("--------------------------------------------------------------------------------");

                    // Dla ka¿dej liczby w¹tków
                    foreach (int threadCount in threadValues)
                    {
                        List<double> iterationTimes = new List<double>();
                        StringBuilder threadResults = new StringBuilder();
                        threadResults.Append($"{threadCount}");

                        // 5 iteracji dla ka¿dej konfiguracji
                        for (int iteration = 1; iteration <= 5; iteration++)
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            var fileGroups = SplitFilesIntoGroups(testFiles, threadCount);
                            ConcurrentDictionary<string, string> testResults = new ConcurrentDictionary<string, string>();
                            List<Task> tasks = new List<Task>();

                            foreach (var group in fileGroups)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    foreach (string filePath in group)
                                    {
                                        try
                                        {
                                            string content = File.ReadAllText(filePath);
                                            string hash = sha256.ComputeHash(content, library);
                                            testResults[filePath] = hash;
                                        }
                                        catch (Exception ex)
                                        {
                                            testResults[filePath] = $"Error: {ex.Message}";
                                        }
                                    }
                                }));
                            }

                            await Task.WhenAll(tasks);
                            sw.Stop();

                            double elapsedMs = sw.Elapsed.TotalMilliseconds;
                            iterationTimes.Add(elapsedMs);
                            threadResults.Append($"\t{elapsedMs:F2}");

                            // Update progress
                            int totalSteps = testFolders.Count * 2 * threadValues.Length * 5; // folders * libraries * threads * iterations
                            int currentStep = (folderIndex * 2 * threadValues.Length * 5) +
                                            ((library == "cs" ? 0 : 1) * threadValues.Length * 5) +
                                            (Array.IndexOf(threadValues, threadCount) * 5) +
                                            iteration;
                            int progress = (currentStep * 100) / totalSteps;
                            progressBar1.Value = Math.Min(progress, 100);
                        }

                        double average = iterationTimes.Average();
                        threadResults.Append($"\t{average:F2}");
                        results.AppendLine(threadResults.ToString());
                    }
                    results.AppendLine();
                }
            }

            // Zapisz wyniki do pliku
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string resultPath = Path.Combine(desktopPath, "TestSHA256Result.txt");
            await File.WriteAllTextAsync(resultPath, results.ToString());

            progressBar1.Value = 100;
            MessageBox.Show("Testing completed. Results saved to TestSHA256Result.txt on desktop.",
                           "Testing Complete",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
