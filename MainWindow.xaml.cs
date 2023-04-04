using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using static CA1_WPF_XAML.Patient;
using static CA1_WPF_XAML.Ward;
using Formatting = Newtonsoft.Json.Formatting;

namespace CA1_WPF_XAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Ward> wards = new ObservableCollection<Ward>();
        private ObservableCollection<Patient> patients = new ObservableCollection<Patient>();
        private string jsonFilePath;

        public object WardCountLabel { get; private set; }
        public IEnumerable<Ward> wardList { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // Initialize list of wards
            wards = new List<Ward>();

            // Load ward data from JSON file
            LoadWards();

            // Load data from file
            try
            {
                string json = File.ReadAllText("savedata.json");
                wards = JsonConvert.DeserializeObject<List<Ward>>(json);
            }
            catch (Exception)
            {
                wards = new List<Ward>();
            }

            // Display data
            lbWards.ItemsSource = wards;
            if (wards.Count > 0)
            {
                WardListBox.SelectedIndex = 0;
            }

            // Bind list boxes to collections
            lbWards.ItemsSource = wards;
            lbPatients.ItemsSource = patients;

            // Select first item in Ward list box
            if (wards.Count > 0)
            {
                lbWards.SelectedIndex = 0;
            }

            DisplayWards();

            // Bind btnSave button Click event to btnSave_Click method
            btnSave.Click += btnSave_Click;

            // Enable/disable Add Ward button based on input
            btnAddWard.IsEnabled = false;
            tbWardName.TextChanged += (s, e) => btnAddWard.IsEnabled = !string.IsNullOrWhiteSpace(tbWardName.Text);

            // Enable/disable Add Patient button based on input
            btnAddPatient.IsEnabled = false;
            tbPatientName.TextChanged += (s, e) => btnAddPatient.IsEnabled = !string.IsNullOrWhiteSpace(tbPatientName.Text) && dpDateOfBirth.SelectedDate != null;
            dpDateOfBirth.SelectedDateChanged += (s, e) => btnAddPatient.IsEnabled = !string.IsNullOrWhiteSpace(tbPatientName.Text) && dpDateOfBirth.SelectedDate != null;
            AButton.Checked += (s, e) => btnAddPatient.IsEnabled = !string.IsNullOrWhiteSpace(tbPatientName.Text) && dpDateOfBirth.SelectedDate != null;
            BButton.Checked += (s, e) => btnAddPatient.IsEnabled = !string.IsNullOrWhiteSpace(tbPatientName.Text) && dpDateOfBirth.SelectedDate != null;
            ABButton.Checked += (s, e) => btnAddPatient.IsEnabled = !string.IsNullOrWhiteSpace(tbPatientName.Text) && dpDateOfBirth.SelectedDate != null;
            OButton.Checked += (s, e) => btnAddPatient.IsEnabled = !string.IsNullOrWhiteSpace(tbPatientName.Text) && dpDateOfBirth.SelectedDate != null;

        }

        private void LoadData()
        {
            try
            {
                // Load test data from JSON file
                string json = File.ReadAllText("testdata.json");

                // Deserialize JSON string to list of wards
                List<Ward> wards = JsonConvert.DeserializeObject<List<Ward>>(json);

                // Display success message
                MessageBox.Show("Data loaded successfully!");

                // Update UI
                DisplayWards();
                DisplayPatients();
                
            }
            catch (Exception ex)
            {
                // Display error message if unable to read or deserialize JSON file
                MessageBox.Show($"Unable to load data from JSON file: {ex.Message}");
            }
            

            // Initialize WardCount
            Ward.WardCount = wards.Count;

            // Add wards to wards list
            wards.AddRange(wards);

            // Clear existing items in listbox
            lbWards.Items.Clear();

            // Add objects to collections
            foreach (Ward ward in wardList)
            {
                wards.Add(ward);
                foreach (Patient patient in ward.Patients)
                {
                    patients.Add(patient);
                }
            }

            // Select first ward in listbox
            lbWards.SelectedIndex = 0;

            try
            {
                // Read JSON string from file
                string json = File.ReadAllText(jsonFilePath);

                // Deserialize JSON string into list of wards
                wards = JsonConvert.DeserializeObject<List<Ward>>(json);

                // Update UI
                DisplayWards();
                DisplayPatients();
                UpdateWardCount();

                // Select first ward in list
                if (wards.Count > 0)
                {
                    lbWards.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Display error message if unable to read JSON string from file
                MessageBox.Show($"Unable to load data from JSON file: {ex.Message}");
            }
        }

        private void DisplayWards()
        {
            // Display list of wards
            lbWards.ItemsSource = wards;
            lbWards.SelectedIndex = 0;

            // Display list of patients in selected ward
            DisplayPatients();
        }

        private void DisplayPatients()
        {
            // Get selected ward
            Ward selectedWard = (Ward)lbWards.SelectedItem;

            // Display list of patients in selected ward
            if (selectedWard != null)
            {
                List<Patient> patients = selectedWard.GetSortedPatients();
                lbPatients.ItemsSource = patients;
                if (patients.Count > 0)
                {
                    lbPatients.SelectedIndex = 0;
                    DisplayPatientDetails();
                }
            }
        }

        private void DisplayPatientDetails()
        {
            // Get selected patient
            Patient selectedPatient = (Patient)lbPatients.SelectedItem;

            // Display details of selected patient
            if (selectedPatient != null)
            {
                tbPatientName.Text = selectedPatient.Name;
                PatientDetailsTextBlock.Text = selectedPatient.BloodType.ToString();
                BloodImage.Source = new BitmapImage(new Uri("images/" + selectedPatient.BloodType.ToString() + ".png", UriKind.Relative));
            }
        }

        private void SaveData()
        {
            try
            {
                // Serialize wards list to JSON
                string json = JsonConvert.SerializeObject(wards, Formatting.Indented);

                // Write JSON to file
                File.WriteAllText("data.json", json);
            }
            catch (Exception ex)
            {
                // Display error message if unable to write JSON string to file
                MessageBox.Show($"Unable to save data to JSON file: {ex.Message}");
            }
        }

        private BloodType GetSelectedBloodType()
        {
            // Determine selected blood type
            if (AButton.IsChecked == true)
            {
                return BloodType.A;
            }
            else if (BButton.IsChecked == true)
            {
                return BloodType.B;
            }
            else if (ABButton.IsChecked == true)
            {
                return BloodType.AB;
            }
            else
            {
                return BloodType.O;
            }
        }

        private void UpdateWardDisplay()
        {
            // Update ward count label
            WardCountLabel.Content = $"Wards ({Ward.WardCount})";

            // Clear patient list box
            lbPatients.ItemsSource = null;

            // Get selected ward
            Ward selectedWard = lbWards.SelectedItem as Ward;

            if (selectedWard != null)
            {
                // Set capacity slider value
                sldWardCapacity.Value = selectedWard.Capacity;

                // Display patients in selected ward
                lbPatients.ItemsSource = selectedWard.Patients;

                // Select first patient
                if (selectedWard.Patients.Count > 0)
                {
                    lbPatients.SelectedIndex = 0;
                }
            }
            else
            {
                // Reset capacity slider value
                sldWardCapacity.Value = 0;
            }
        }

        private void UpdatePatientDisplay()
        {
            // Clear details text block and image
            PatientDetailsTextBlock.Text = "";
            BloodImage.Source = null;

            // Get selected patient
            Patient selectedPatient = lbPatients.SelectedItem as Patient;

            if (selectedPatient != null)
            {
                // Set details text block text
                PatientDetailsTextBlock.Text = $"{selectedPatient.Name}\n{selectedPatient.Age} {selectedPatient.BloodType}";

                // Set image source
                string imagePath = $"images/{selectedPatient.Patient.BloodType}.png";
                if (File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                    BloodImage.Source = bitmap;
                }
            }
        }

        //Event handlers

        // Load event handler for MainWindow
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Select first ward in list
            if (lbWards.Items.Count > 0)
            {
                lbWards.SelectedIndex = 0;
            }
        }

        private void lbWards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update display for selected Ward
            UpdateWardDisplay();
            UpdatePatientDisplay();

            // Clear selection in PatientListBox and Details
            lbPatients.SelectedItem = null;
            BloodImage.Source = null;

            // Update capacity display
            if (lbWards.SelectedItem is Ward selectedWard)
            {
                sldWardCapacity.Content = "Capacity: " + selectedWard.Capacity;
                lbPatients.ItemsSource = selectedWard.Patients;
                lbPatients.SelectedIndex = 0;
            }

            DisplayPatients();
        }
        

        private void lbPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Display details of selected patient
            Patient selectedPatient = lbPatients.SelectedItem as Patient;

            // Update display for selected Patient
            UpdatePatientDisplay();

            if (selectedPatient != null)
            {
                // Set details text block text
                PatientDetailsTextBlock.Text = $"{selectedPatient.Name}\n{selectedPatient.Age} {selectedPatient.Patient.BloodType}";

                // Set image source
                string imagePath = $"images/{selectedPatient.Patient.BloodType}.png";
                if (File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                    BloodImage.Source = bitmap;
                }
                else
                {
                    BloodImage.Source = null;
                }
            }

            DisplayPatientDetails();

        }

        private void tbWardName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable Add Ward button if ward name is not empty
            btnAddWard.IsEnabled = !string.IsNullOrEmpty(tbWardName.Text);
        }

        private void tbPatientName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable Add Patient button if patient name is not empty
            btnAddPatient.IsEnabled = !string.IsNullOrEmpty(tbPatientName.Text);
        }

        private void btnAddWard_Click(object sender, RoutedEventArgs e)
        {
               
            // Create new Ward object
            Ward newWard = new Ward(tbWardName.Text, (int)sldWardCapacity.Value);

            // Add Ward to collection and select it
            wards.Add(newWard);
            lbWards.SelectedItem = newWard;

            // Refresh display
            lbWards.ItemsSource = null;
            lbWards.ItemsSource = wards;
            lbWards.SelectedItem = newWard;

            // Clear text box
            tbWardName.Text = "";
            sldWardCapacity.Value = 1;

            // Disable button
            btnAddWard.IsEnabled = false;

            // Display updated list of wards
            DisplayWards();
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            // Get name and blood type from textboxes
            string name = tbPatientName.Text.Trim();
            BloodType bloodType = (BloodType)Enum.Parse(typeof(BloodType), (string)((RadioButton)spBloodType.Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true)).Content);

            // Get date of birth from DatePicker
            DateTime dateOfBirth = dpDateOfBirth.SelectedDate.GetValueOrDefault();

            // Get selected ward
            Ward selectedWard = (Ward)lbWards.SelectedItem;

            // Check if a ward is selected
            if (selectedWard == null)
            {
                MessageBox.Show("Please select a ward to add the patient to.");
                return;
            }

            // Check if the selected ward is full
            if (selectedWard.IsFull())
            {
                MessageBox.Show("The selected ward is full. Please select another ward to add the patient to.");
                return;
            }

            // Create new patient
            Patient newPatient = new Patient(name, dateOfBirth, bloodType, selectedWard);

            // Add patient to selected ward
            selectedWard.Patients.Add(newPatient);

            // Refresh patient list display
            lbPatients.ItemsSource = null;
            lbPatients.ItemsSource = selectedWard.Patients;

            // Select the new patient in the list
            lbPatients.SelectedItem = newPatient;

            // Clear textboxes
            tbPatientName.Text = "";
            dpDateOfBirth.SelectedDate = null;
            foreach (RadioButton rb in spBloodType.Children.OfType<RadioButton>())
            {
                rb.IsChecked = false;
            }


        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Serialize objects to JSON
            string json = JsonConvert.SerializeObject(wards, Formatting.Indented);

            // Write JSON to file
            File.WriteAllText("testdata.json", json);

            // Save data to JSON file
            SaveData();
            MessageBox.Show("Data saved successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load test data from file
            string json = File.ReadAllText("testdata.json");
            wards = JsonConvert.DeserializeObject<List<Ward>>(json);

            // Select first Ward and update display
            if (wards.Count > 0)
            {
                lbWards.SelectedItem = wards[0];
            }
            UpdateWardDisplay();
            UpdatePatientDisplay();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();

            // Serialize and save data to file
            string json = JsonConvert.SerializeObject(wards);
            File.WriteAllText("savedata.json", json);

            // Show confirmation message
            MessageBox.Show("Data saved.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();

            try
            {
                // Load data from file
                string json = File.ReadAllText("savedata.json");
                wards = JsonConvert.DeserializeObject<List<Ward>>(json);

                // Update Ward count
                Ward.WardCount = wards.Count;

                // Refresh display
                lbWards.ItemsSource = null;
                lbWards.ItemsSource = wards;
                if (wards.Count > 0)
                {
                    lbWards.SelectedIndex = 0;
                }

                // Save data to file
                try
                {
                    File.WriteAllText("savedata.json", JsonConvert.SerializeObject(wards));

                    MessageBox.Show("Data saved successfully.", "Save Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error saving data to file.", "Save Data", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
    }

        private void lbPatients_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void lbWards_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}


