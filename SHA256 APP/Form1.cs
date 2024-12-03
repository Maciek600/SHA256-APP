using System.Collections.Concurrent;
using System.Text;

namespace SHA256_APP
{
    public partial class Form1 : Form
    {
        private string[] inputFilePaths = Array.Empty<string>(); // Tablica z nazwami wybranych plik�w

        private int threadCount = 1; //domy�lna warto�� na razie


        // Tablica warto�ci odpowiadaj�cych pozycjom TrackBar
        private readonly int[] threadValues = { 1, 2, 4, 8, 16, 32, 64 };
        public Form1()
        {
            InitializeComponent();
            // Inicjalizacja TrackBar
            trackBar1.Minimum = 0; // Indeks w tablicy threadValues
            trackBar1.Maximum = threadValues.Length - 1; // Ostatni indeks
            trackBar1.Value = 0; // Domy�lna warto�� (pierwszy element: 1)

            // Wy�wietlenie warto�ci na starcie
            labelTrackBarValue.Text = threadValues[trackBar1.Value].ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Tworzymy okno dialogowe
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Ustawienia dialogu
                openFileDialog.Title = "Select Input Files";
                openFileDialog.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt"; // Filtry plik�w
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Domy�lna lokalizacja
                openFileDialog.Multiselect = true; // Pozwala na wyb�r wielu plik�w

                // Sprawdzenie, czy u�ytkownik wybra� pliki
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Pobranie �cie�ek do wybranych plik�w
                    inputFilePaths = openFileDialog.FileNames;

                    // Wy�wietlenie wszystkich wybranych plik�w (dla debugowania lub informacji)
                    string fileList = string.Join("\n", inputFilePaths);
                    MessageBox.Show($"Selected files:\n{fileList}", "Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            threadCount = threadValues[trackBar1.Value];  // Ustawienie odpowiedniej liczby w�tk�w
            labelTrackBarValue.Text = threadCount.ToString();  // Wy�wietlenie warto�ci na etykiecie
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
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

            string selectedLibrary = radioButton1.Checked ? "asm" : "cs";
            int selectedThreads = threadValues[trackBar1.Value]; // Liczba w�tk�w wybrana z TrackBar

            // Podzia� plik�w na grupy
            var fileGroups = SplitFilesIntoGroups(inputFilePaths, selectedThreads);

            // Kolekcja wynik�w
            ConcurrentDictionary<string, string> results = new ConcurrentDictionary<string, string>();

            // Przygotowanie paska post�pu
            progressBar1.Minimum = 0;
            progressBar1.Maximum = inputFilePaths.Length;
            progressBar1.Value = 0;

            // Lista zada� Task
            List<Task> tasks = new List<Task>();

            foreach (var group in fileGroups)
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (string filePath in group)
                    {
                        string content = File.ReadAllText(filePath);
                        string hash = SHA256App.SHA256.ComputeHash(content, selectedLibrary);
                        results[filePath] = hash;

                        // Zapis pliku wyj�ciowego na pulpicie
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath);
                        string outputFilePath = Path.Combine(desktopPath, $"{fileName}X{extension}");
                        File.WriteAllText(outputFilePath, hash);

                        // Aktualizacja paska post�pu w w�tku GUI
                        Invoke(new Action(() => progressBar1.Value++));
                    }
                }));
            }

            // Oczekiwanie na zako�czenie wszystkich zada�
            Task.WaitAll(tasks.ToArray());

            // Wy�wietlenie wynik�w
            StringBuilder sb = new StringBuilder();
            foreach (var result in results)
            {
                sb.AppendLine($"File: {result.Key}\nHash: {result.Value}\n");
            }

            MessageBox.Show(sb.ToString(), "Hashes Computed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private List<List<string>> SplitFilesIntoGroups(string[] filePaths, int numberOfGroups)
        {
            List<List<string>> groups = new List<List<string>>();
            for (int i = 0; i < numberOfGroups; i++)
            {
                groups.Add(new List<string>());
            }

            for (int i = 0; i < filePaths.Length; i++)
            {
                groups[i % numberOfGroups].Add(filePaths[i]);
            }

            return groups;
        }


        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
