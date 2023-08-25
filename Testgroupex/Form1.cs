using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testgroupex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadDataGridView();
        }

        private void Form1_load(object sender, EventArgs e)
        {
            this.FormClosing += Form1_FormClosing;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Bổ sung chức năng nâng cao
            DialogResult result = MessageBox.Show("Bạn chắc chắn muốn thoát?", "Thông báo", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        private DataTable dataTable = new DataTable();
        private void LoadDataGridView()
        {
            dataTable.Columns.Add("Tên cô dâu chú rể", typeof(string));
            dataTable.Columns.Add("Ngày tổ chức", typeof(string));
            dataTable.Columns.Add("Địa điểm", typeof(string));
            dataTable.Columns.Add("Số lượng khách mời", typeof(int));

            dataGridView1.DataSource = dataTable;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(textBox4.Text, out a))
            {
                dataTable.Rows.Add(textBox1.Text, textBox2.Text, textBox3.Text, a);
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox1.Focus();
                SortDataTableByWeddingDate();

            }
            else
            {
                MessageBox.Show("Invalid input for 'Số lượng khách mời'. Please enter a valid number.", "Warning");
                textBox4.Text = "";
                textBox4.Focus();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string a = textBox1.Text;
            for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dataTable.Rows[i];
                if (row["Tên cô dâu chú rể"].ToString().ToLower() == a.ToLower())
                {
                    DialogResult b = MessageBox.Show("Bạn có chắc chắn muốn xóa tiệc cưới này?", "Thông báo", MessageBoxButtons.YesNo);
                    if (b == DialogResult.Yes)
                    {
                        dataTable.Rows.Remove(row);
                        textBox1.Text = "";
                        textBox1.Focus();
                        break;
                    }
                    else
                    {
                        textBox1.Text = "";
                        textBox1.Focus();
                        break;
                    }

                }
                else
                {

                    textBox1.Text = "";
                    textBox1.Focus();


                }
            }
        }
        //Bổ sung chức năng nâng cao 
        private void SortDataTableByWeddingDate()
        {
            DataView dv = dataTable.DefaultView;
            dv.Sort = "Ngày tổ chức";

            List<DataRow> rows = dv.ToTable().Rows.Cast<DataRow>().ToList();
            rows.Sort(new CustomDateComparer());

            dataTable.Clear();
            foreach (DataRow row in rows)
            {
                dataTable.Rows.Add(row.ItemArray);
            }

            dataGridView1.DataSource = dataTable;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string userInputStr = InputDialog.ShowDialog("Bạn muốn cập nhật dòng nào: ", "Thông báo");

            if (int.TryParse(userInputStr, out int userInput) && userInput >= 1 && userInput <= dataTable.Rows.Count)
            {
                DataRow row = dataTable.Rows[userInput - 1];

                int choice = -1;
                while (choice != 5)
                {
                    choice = int.Parse(InputDialog.ShowDialog(@"Lựa chọn:
1. Tên cô dâu chú rể
2. Ngày tổ chức
3. Địa điểm
4. Số lượng khách mời
5. Hoàn thành cập nhật", "Thông báo"));

                    switch (choice)
                    {
                        case 1:
                            string newName = InputDialog.ShowDialog("Nhập tên cô dâu chú rể mới: ", "Cập nhật");
                            row["Tên cô dâu chú rể"] = newName;
                            break;
                        case 2:
                            string newDate = InputDialog.ShowDialog("Nhập ngày tổ chức mới [ngày-tháng-năm]: ", "Cập nhật");
                            row["Ngày tổ chức"] = newDate;
                            break;
                        case 3:
                            string newLocation = InputDialog.ShowDialog("Nhập địa điểm mới: ", "Cập nhật");
                            row["Địa điểm"] = newLocation;
                            break;
                        case 4:
                            string newNumberGuest = InputDialog.ShowDialog("Nhập số lượng khách mời mới: ", "Cập nhật");
                            if (int.TryParse(newNumberGuest, out int numberGuest))
                            {
                                row["Số lượng khách mời"] = numberGuest;
                            }
                            else
                            {
                                MessageBox.Show("Invalid input for 'Số lượng khách mời'. Please enter a valid number.", "Warning");
                            }
                            break;
                        case 5:
                            SortDataTableByWeddingDate(); // Sort the table after completing updates
                            break;
                        default:
                            MessageBox.Show("Invalid choice, please enter again!", "Thông báo");
                            break;
                    }
                }

                SortDataTableByWeddingDate(); 
            }
            else if (!string.IsNullOrEmpty(userInputStr)) 
            {
                MessageBox.Show("Invalid input. Please enter a valid row number.", "Thông báo");
            }
        }




    }
    class CustomDateComparer : IComparer<DataRow>
    {
        public int Compare(DataRow x, DataRow y)
        {
            DateTime date1 = DateTime.ParseExact(x["Ngày tổ chức"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(y["Ngày tổ chức"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            int yearComparison = date1.Year.CompareTo(date2.Year);
            if (yearComparison != 0)
            {
                return yearComparison;
            }

            int monthComparison = date1.Month.CompareTo(date2.Month);
            if (monthComparison != 0)
            {
                return monthComparison;
            }

            return date1.Day.CompareTo(date2.Day);
        }
    }



    static class InputDialog
    {
        private static string inputValue = "";

        public static string ShowDialog(string prompt, string title)
        {
            int formWidth = 400;  
            int formHeight = 200; 

            Form promptForm = new Form()
            {
                Width = formWidth,
                Height = formHeight,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label promptLabel = new Label()
            {
                Left = 20,       
                Top = 5,       
                Width = formWidth - 40,    
                Height = formHeight - 80,  
                Text = prompt
            };

            TextBox textBox = new TextBox()
            {
                Left = 20,       
                Top = 85,        
                Width = formWidth - 40
            };

            Button confirmation = new Button()
            {
                Text = "OK",
                Left = formWidth / 2 - 30,
                Width = 60,
                Top = formHeight - 80 
            };

            confirmation.Click += (sender, e) =>
            {
                inputValue = textBox.Text;
                promptForm.Close();
            };

            textBox.TextChanged += (sender, e) =>
            {
                inputValue = textBox.Text;
            };

            promptForm.Controls.Add(textBox);
            promptForm.Controls.Add(confirmation);
            promptForm.Controls.Add(promptLabel);
            promptForm.AcceptButton = confirmation;

            promptForm.ShowDialog();

            return inputValue;
        }
    }
}


