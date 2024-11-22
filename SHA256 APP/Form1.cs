namespace SHA256_APP
{
    public partial class Form1 : Form
    {
        private string[] inputFilePaths = Array.Empty<string>(); // Tablica z nazwami wybranych plik�w


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
                    string[] filePaths = openFileDialog.FileNames;

                    // Wy�wietlenie wszystkich wybranych plik�w (dla debugowania lub informacji)
                    string fileList = string.Join("\n", filePaths);
                    MessageBox.Show($"Selected files:\n{fileList}", "Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mo�esz zapisa� �cie�ki do zmiennej globalnej lub przetwarza� je dalej
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Wy�wietlanie aktualnej warto�ci na podstawie indeksu
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
            // 1. Sprawdzenie, czy wybrano pliki wej�ciowe
            if (inputFilePaths.Length == 0)
            {
                MessageBox.Show("Please select at least one input file.", "Input File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Sprawdzenie, czy wybrano bibliotek� (RadioButton)
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Please select a library (ASM or x64).", "Library Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Pobranie ilo�ci w�tk�w
            int selectedThreads = threadValues[trackBar1.Value];

            // 4. Sprawdzenie, kt�ra biblioteka zosta�a wybrana
            string selectedLibrary = radioButton1.Checked ? "ASM" : "x64";

            // 5. Wy�wietlenie wynik�w (lub przekazanie ich do innej funkcji)
            string message = $"Starting app with:\n" +
                             $"- Selected Library: {selectedLibrary}\n" +
                             $"- Number of Threads: {selectedThreads}\n" +
                             $"- Input Files: {string.Join(", ", inputFilePaths)}";

            MessageBox.Show(message, "App Started", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Mo�esz tu wywo�a� logik� przetwarzania danych
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
