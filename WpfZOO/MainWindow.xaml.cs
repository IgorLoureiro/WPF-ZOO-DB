using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WpfZOO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["WpfZOO.Properties.Settings.ZooDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAnimals();
        }

        private void ShowZoos()
        {
            try
            {
                string query = "select * from Zoo";
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    //Which Information of the Table in DataTable should be shown in our ListBox?
                    ListZoos.DisplayMemberPath = "Location";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    ListZoos.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    ListZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = "select * from Animal a inner join ZooAnimal " +
                    "za on a.Id = za.AnimalId where za.ZooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);

                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);

                    //Which Information of the Table in DataTable should be shown in our ListBox?
                    ListAssociatedAnimals.DisplayMemberPath = "Name";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    ListAssociatedAnimals.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    ListAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }

        }

        private void ListZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            ShowSelectedZooInTextBox();
        }

        private void ShowAnimals()
        {
            try
            {
                string query = "select * from Animal";
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable AnimalTable = new DataTable();

                    sqlDataAdapter.Fill(AnimalTable);

                    //Which Information of the Table in DataTable should be shown in our ListBox?
                    Animals.DisplayMemberPath = "Name";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    Animals.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    Animals.ItemsSource = AnimalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void Delete_Zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                sqlCommand.ExecuteScalar();

                sqlConnection.Close();
                ShowZoos();
            } catch (Exception ex){
                MessageBox.Show(ex.ToString());
            } finally { 
                sqlConnection.Close();
                ShowZoos();
            }
            
        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", TextBox.Text);
                sqlCommand.ExecuteScalar();

                sqlConnection.Close();
                ShowZoos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void addAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", Animals.SelectedValue);
                sqlCommand.ExecuteScalar();

                sqlConnection.Close();
                ShowZoos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
                ShowAssociatedAnimals();
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", TextBox.Text);
                sqlCommand.ExecuteScalar();

            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }

            
        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", Animals.SelectedValue);
                sqlCommand.ExecuteScalar();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }

        private void ShowSelectedZooInTextBox()
        {
            try
            {
                string query = "select Location from Zoo where Id = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);

                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    TextBox.Text = zooTable.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void Animals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimal();
        }

        private void ShowSelectedAnimal()
        {
            string query = "select Name from Animal where Id = @AnimalId";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            using (sqlDataAdapter)
            {

                sqlCommand.Parameters.AddWithValue("@AnimalId", Animals.SelectedValue);

                DataTable AnimalsTable = new DataTable();

                sqlDataAdapter.Fill(AnimalsTable);

                TextBox.Text = AnimalsTable.Rows[0]["Name"].ToString();
            }
        }
        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", TextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }
        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Animal Set Name = @Name where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", Animals.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", TextBox.Text);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
                ShowAnimals();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }
    }

}
