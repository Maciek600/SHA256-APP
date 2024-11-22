namespace SHA256_APP
{
    public partial class Form1 : Form
    {
        private string[] inputFilePaths = Array.Empty<string>(); // Tablica z nazwami wybranych plików


        // Tablica wartoœci odpowiadaj¹cych pozycjom TrackBar
        private readonly int[] threadValues = { 1, 2, 4, 8, 16, 32, 64 };
        public Form1()
        {
            InitializeComponent();
            // Inicjalizacja TrackBar
            trackBar1.Minimum = 0; // Indeks w tablicy threadValues
            trackBar1.Maximum = threadValues.Length - 1; // Ostatni indeks
            trackBar1.Value = 0; // Domyœlna wartoœæ (pierwszy element: 1)

            // Wyœwietlenie wartoœci na starcie
            labelTrackBarValue.Text = threadValues[trackBar1.Value].ToString();
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
                    string[] filePaths = openFileDialog.FileNames;

                    // Wyœwietlenie wszystkich wybranych plików (dla debugowania lub informacji)
                    string fileList = string.Join("\n", filePaths);
                    MessageBox.Show($"Selected files:\n{fileList}", "Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mo¿esz zapisaæ œcie¿ki do zmiennej globalnej lub przetwarzaæ je dalej
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Wyœwietlanie aktualnej wartoœci na podstawie indeksu
            labelTrackBarValue.Text = threadValues[trackBar1.Value].ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 1. Sprawdzenie, czy wybrano pliki wejœciowe
            if (inputFilePaths.Length == 0)
            {
                MessageBox.Show("Please select at least one input file.", "Input File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Sprawdzenie, czy wybrano bibliotekê (RadioButton)
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Please select a library (ASM or x64).", "Library Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Pobranie iloœci w¹tków
            int selectedThreads = threadValues[trackBar1.Value];

            // 4. Sprawdzenie, która biblioteka zosta³a wybrana
            string selectedLibrary = radioButton1.Checked ? "ASM" : "x64";

            // 5. Wyœwietlenie wyników (lub przekazanie ich do innej funkcji)
            string message = $"Starting app with:\n" +
                             $"- Selected Library: {selectedLibrary}\n" +
                             $"- Number of Threads: {selectedThreads}\n" +
                             $"- Input Files: {string.Join(", ", inputFilePaths)}";

            MessageBox.Show(message, "App Started", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Mo¿esz tu wywo³aæ logikê przetwarzania danych
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
