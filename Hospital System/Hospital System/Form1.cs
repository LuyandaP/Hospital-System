using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Hospital_System
{
    public partial class Form1 : Form
    {
        //Connection string to database  
        OleDbConnection cs = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Luyanda\Documents\hospitaldb.accdb;Persist Security Info=False");
        public Form1()
        {
            InitializeComponent();
            //Event handler for printing
            printdoc1.PrintPage += new PrintPageEventHandler(printdoc1_PrintPage);
        }
        
        //Global Variables
        int eid;
        string password;
        string position;
        string eName;
        int ptntID;
        string ptntName;
        string ptntLName;
        int emContact;
        string emConName;
        string emConLName;
        string street;
        string city;
        string country;
        string ward;
        int bednumber;
        string adminBy;
        string item;
        double quantity;
        string treatedby;
        string diagnosis;
        DateTime date;
        DateTime dateIn;
        DateTime dateOut;

        //Object Variables for Database
        OleDbCommand cmd;
        OleDbDataReader r;
        OleDbDataAdapter gv;

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                eid = Convert.ToInt32(txtUsername.Text);
                password = txtPassword.Text;
            
            
                //Entry of login information
                string sql = $"select * from Staff where EmployeeID={eid} and Password='{password}'";

                int count = 0;

                cs.Open();

                cmd = new OleDbCommand(sql, cs);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                r = cmd.ExecuteReader();

                //verifying account
                while (r.Read())
                {
                    count = count + 1;
                    eName = r["EmployeeName"].ToString();
                    position = r["Position"].ToString();
                }
                cs.Close();

                if (count == 1)
                {
                    //Differnt Accounts
                    if (position == "Reception")
                    {
                        panLogin.Hide();
                        panPatientInfo.Show();
                        MessageBox.Show($"Welcome {eName}");
                        txtUsername.Clear();
                        txtPassword.Clear();
                    }
                    else if (position == "Nurse")
                    {
                        panLogin.Hide();
                        panPatientExp.Show();
                        MessageBox.Show($"Welcome {eName}");
                        txtUsername.Clear();
                        txtPassword.Clear();
                        txtItemConvertion.Items.Add("ml");
                        txtItemConvertion.Items.Add("mg");
                        txtItemConvertion.Items.Add("unit");
                    }
                    else if (position == "Accounts")
                    {
                        panLogin.Hide();
                        panBilling.Show();
                        MessageBox.Show($"Welcome {eName}");
                        txtUsername.Clear();
                        txtPassword.Clear();
                    }
                    else if (position == "Doctor")
                    {
                        panLogin.Hide();
                        panDiagnosisnPersriptions.Show();
                        MessageBox.Show($"Welcome {eName}");
                        txtUsername.Clear();
                        txtPassword.Clear();
                        txtMedConvertion.Items.Add("ml");
                        txtMedConvertion.Items.Add("mg");
                        txtMedConvertion.Items.Add("unit");
                    }
                    else
                    {
                        MessageBox.Show("No Authorizations");
                    }

                }
                else
                {
                    label44.Text = "Incorrect Employee ID or Password";
                }
            }
            catch (Exception ex)
            {
                label44.Text = "Enter in correct login details";
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                ptntID = Convert.ToInt32(txtPatientID.Text);
                ptntName = txtPatientName.Text;
                ptntLName = txtPatientLName.Text;
                emContact = Convert.ToInt32(txtEmContact.Text);
                emConName = txtEmConName.Text;
                emConLName = txtEmConLName.Text;
                street = txtStreet.Text;
                city = txtCity.Text;
                country = txtCountry.Text;
                date = Convert.ToDateTime(txtDate.Text);

                //entering patient info for outpatients to database
                string sql = $"insert into OutPatients(PatientID,PatientName,PatientLastName,EmergencyContact,EmergencyContactName,EmergencyContactLastName,Street,City,Country,DateIn) values('{ptntID}','{ptntName}','{ptntLName}','{emContact}','{emConName}','{emConLName}','{street}','{city}','{country}','{date}')";
                cmd = new OleDbCommand();

                cs.Open();

                cmd.Connection = cs;
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                cs.Close();

                MessageBox.Show("Information Entered Successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Enter in all patient information");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnAdmit_Click(object sender, EventArgs e)
        {
            try
            {
                ptntID = Convert.ToInt32(txtPatientIDIn.Text);
                ptntName = txtPatientNameIn.Text;
                ptntLName = txtPatientLNameIn.Text;
                ward = txtWard.Text;
                bednumber = Convert.ToInt32(txtBedNo.Text);
                adminBy = txtAdmittedBy.Text;
                dateIn = Convert.ToDateTime(txtDateIn.Text);

                //entering patient info for inpatients to database
                string sql = $"insert into InPatient(PatientID,PatientName,PatientLastName,Ward,BedNumber,DateIn,AdmittedBy) values('{ptntID}','{ptntName}','{ptntLName}','{ward}','{bednumber}','{dateIn}','{adminBy}')";
                cmd = new OleDbCommand();

                cs.Open();

                cmd.Connection = cs;
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                cs.Close();

                MessageBox.Show("Information Entered Successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Incorrect Information");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtPatientIDSrch.Text == "")
                {
                    txtPatientIDSrch.Text = "0";
                }

                string sql1 = $"select * from OutPatients where PatientID={txtPatientIDSrch.Text} or (PatientName='{txtPatientNameSrch.Text}' and PatientLastName='{txtPatientLNameSrch.Text}')";
                string sql2 = $"select * from InPatient where PatientID={txtPatientIDSrch.Text} or (PatientName='{txtPatientNameSrch.Text}' and PatientLastName='{txtPatientLNameSrch.Text}')";

                gv = new OleDbDataAdapter(sql1, cs);

                OleDbDataAdapter gv2 = new OleDbDataAdapter(sql2, cs);

                cs.Open();

                //displaying Outpatient Info from database
                DataTable data1 = new DataTable();
                gv.Fill(data1);
                dataGridView1.DataSource = data1;

                cs.Close();

                cs.Open();

                //displaying Inpatient Info from database
                DataTable data2 = new DataTable();
                gv2.Fill(data2);
                dataGridView2.DataSource = data2;

                cs.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Not Enough Information");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnDischarge_Click(object sender, EventArgs e)
        {
            try
            {
                dateOut = Convert.ToDateTime(txtDateOut.Text);

                //Inputs the Departure date for inpatients to database
                string sql = $"update InPatient set DateOut='{dateOut}' where PatientID={txtPatientIDSrch.Text}";

                cmd = new OleDbCommand(sql, cs);

                cs.Open();

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                cs.Close();
                MessageBox.Show("Updated");
            }
            catch(Exception ex)
            {
                MessageBox.Show("No date entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //Adding patient diagnosis to database
                string sql = $"update OutPatients set Diagnosis='{txtDiagnosis.Text}',Treatedby='{eName}' where PatientID={txtPatientIDUp.Text}";

                cmd = new OleDbCommand(sql, cs);

                cs.Open();

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                cs.Close();

                MessageBox.Show("Diagnosis submitted");
            }
            catch(Exception ex)
            {
                MessageBox.Show("No Diagnosis entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnEnterMeds_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlr = $"select * from OutPatients where PatientID={txtPatientIDUp.Text}";

                cmd = new OleDbCommand(sqlr, cs);

                cs.Open();

                cmd.CommandText = sqlr;

                r = cmd.ExecuteReader();

                if (r.Read())
                {
                    ptntName = r["PatientName"].ToString();
                    ptntLName = r["PatientLastName"].ToString();        //Extracting patient info for perscription
                }
                else
                {
                    MessageBox.Show("No Entries");
                }

                cs.Close();

                //Entering Perscription to database
                string sqlin = $"insert into Medication(PatientID,PatientName,PatientLastName,Perscription,Amount,Convertion,PerscribedBy,Instructions) values('{txtPatientIDUp.Text}','{ptntName}','{ptntLName}','{txtMedication.Text}','{txtMedAmount.Value}','{txtMedConvertion.Text}','{eName}','{txtInstructions.Text}')";

                cmd = new OleDbCommand(sqlin, cs);

                cs.Open();

                cmd.CommandText = sqlin;
                cmd.ExecuteNonQuery();

                cs.Close();

                MessageBox.Show("Perscription entry successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnUpdateExpenses_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlr = $"select * from OutPatients where PatientID={txtPatientIDExpenses.Text}";

                cmd = new OleDbCommand(sqlr, cs);

                cs.Open();

                cmd.CommandText = sqlr;

                r = cmd.ExecuteReader();

                if (r.Read())
                {
                    ptntName = r["PatientName"].ToString();
                    ptntLName = r["PatientLastName"].ToString();
                }
                else
                {
                    MessageBox.Show("No Entries");
                }

                cs.Close();

                //Enter patient expenses to database
                string sqlin = $"insert into Expenses(PatientID,PatientName,PatientLastName,Item,Quantity,Convertion) values('{txtPatientIDExpenses.Text}','{ptntName}','{ptntLName}','{txtItem.Text}','{txtItemAmount.Value}','{txtItemConvertion.Text}')";

                cmd = new OleDbCommand(sqlin, cs);

                cs.Open();

                cmd.CommandText = sqlin;
                cmd.ExecuteNonQuery();

                cs.Close();

                MessageBox.Show("Expense entry successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnSearchExpenses_Click(object sender, EventArgs e)
        {
            try
            {
                //Search for patients expenses
                string sql = $"select PatientID,PatientName,PatientLastName,Item,Quantity,Convertion from Expenses where PatientID={txtPatientIDExpSrch.Text}";

                gv = new OleDbDataAdapter(sql, cs);

                cs.Open();

                DataTable data = new DataTable();
                gv.Fill(data);
                dataGridView3.DataSource = data;

                cs.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnPerscriptionSrch_Click(object sender, EventArgs e)
        {
            label43.Text = "";
            int count = 0;

            try
            {
                //Retrieve information for patient perscriptions
                string sql = $"select PatientID,PatientName,PatientLastName,Perscription,Amount,Convertion,PerscribedBy,Instructions from Medication where PatientID={txtPatientIDPersciption.Text}";

                cmd = new OleDbCommand(sql, cs);

                cs.Open();

                cmd.CommandText = sql;

                r = cmd.ExecuteReader();

                while (r.Read())
                {
                    //Display Perscription
                    label43.Text += $"ID: {r["PatientID"]} \n First Name: {r["PatientName"].ToString()} \n Last Name: {r["PatientLastName"].ToString()} \n Perscription: {r["Perscription"].ToString()} {r["Amount"]} {r["Convertion"].ToString()} \n Perscribed by: {r["PerscribedBy"].ToString()} \n Instructions: {r["Instructions"].ToString()} \n\n";

                    count++;
                }

                if (count == 0)
                {
                    label43.Text = "No perscriptions for this ID";
                }
                cs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnSearchBill_Click(object sender, EventArgs e)
        {
            label35.Text = "";

            Int32 count = 0;
            double total = 0;
            double bedding = 0;
            double bedprice = 0;

            try
            {
                string sqldate = $"select DateIn,DateOut from InPatient where PatientID={txtPatientIDBillSrch.Text}";

                cmd = new OleDbCommand(sqldate, cs);

                cs.Open();

                cmd.CommandText = sqldate;

                r = cmd.ExecuteReader();

                if (r.Read())
                {
                    //Extracts table cell data into variables
                    dateIn = Convert.ToDateTime(r["DateIn"].ToString());
                    dateOut = Convert.ToDateTime(r["DateOut"].ToString());
                }
                else
                {
                    MessageBox.Show("No Entries");
                }
                cs.Close();

                //Contains calculations for dates
                DateCalculation dates = new DateCalculation(dateIn, dateOut);

                //Retrieve price for bed
                string sqlbed = $"select * from Prices where Item='Bed'";

                cmd = new OleDbCommand(sqlbed, cs);
                gv = new OleDbDataAdapter(sqldate, cs);

                cs.Open();

                //Display inpatient dates
                DataTable data = new DataTable();
                gv.Fill(data);
                dataGridView5.DataSource = data;

                cs.Close();

                cs.Open();

                cmd.CommandText = sqlbed;

                r = cmd.ExecuteReader();

                if (r.Read())
                {
                    bedprice = Convert.ToDouble(r["Price"]);
                }
                else
                {
                    MessageBox.Show("No Entries");
                }
                cs.Close();

                string sqlexpenses = $"select count(*) from Expenses where PatientID={txtPatientIDBillSrch.Text}";

                //Display patient info for expneses
                string sqlexpensessdb = $"select distinct PatientName,PatientLastName from Expenses where PatientID={txtPatientIDBillSrch.Text}";

                gv = new OleDbDataAdapter(sqlexpensessdb, cs);

                cs.Open();

                //Display Patient info
                data = new DataTable();
                gv.Fill(data);
                dataGridView6.DataSource = data;

                cs.Close();

                cmd = new OleDbCommand(sqlexpenses, cs);

                cs.Open();

                cmd.CommandText = sqlexpenses;

                //Row count to verify if there if patient info and how much there is
                count = Convert.ToInt32(cmd.ExecuteScalar());

                cs.Close();

                if (count > 0)
                {
                    bedding = bedprice * dates.days();      //Cost for bed

                    label35.Text = $"Bedding    R{bedding} \n";
                }

                int id = 0;
                double price = 0;

                for (int i = 0; i < count; i++)
                {
                    string sqlvalues = $"select * from Expenses where PatientID={txtPatientIDBillSrch.Text} and ID>{id}";

                    cmd = new OleDbCommand(sqlvalues, cs);

                    cs.Open();

                    r = cmd.ExecuteReader();

                    if (r.Read())
                    {
                        item = r["Item"].ToString();
                        quantity = Convert.ToDouble(r["Quantity"]);
                        id = Convert.ToInt32(r["ID"]);
                    }
                    else
                    {
                        MessageBox.Show("No entries found");
                        cs.Close();
                        continue;
                    }

                    cs.Close();

                    //Retrive pricing information for medication 
                    string sqlprices = $"select * from Prices where Item='{item}'";

                    cmd = new OleDbCommand(sqlprices, cs);

                    cs.Open();

                    r = cmd.ExecuteReader();

                    if (r.Read())
                    {
                        price = Convert.ToDouble(r["Price"]);
                    }
                    else
                    {
                        MessageBox.Show("No Connection");
                    }

                    cs.Close();

                    //Calculation for medication
                    label35.Text += $"{item}     R{price * quantity} \n";
                    total += price * quantity;
                }

                label35.Text += $"Total     R{total + bedding}";    //Cost of Medication while admitted
            }
            catch(Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnSignoutDiagnosisnPerscriptions_Click(object sender, EventArgs e)
        {
            //Loop to empty all controls in the panel
            foreach (Control c in panDiagnosisnPersriptions.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.Text = "";
                }
                else if (c.GetType() == typeof(NumericUpDown))
                {
                    c.Text = "";
                }
                else if(c.GetType() == typeof(ComboBox))
                {
                    c.Text = "";
                    ((ComboBox)c).Items.Clear();
                }
            }
            //return to login
            panDiagnosisnPersriptions.Hide();
            panLogin.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panAdmitFrm.Hide();
            panBilling.Hide();
            panDiagnosisnPersriptions.Hide();
            panPatientExp.Hide();
            panPatientInfo.Hide();
            panPerscriptionsSrch.Hide();
            panSrchnDisChrg.Hide();
            panPatientHistory.Hide();
        }

        private void btnSignoutPInfo_Click(object sender, EventArgs e)
        {
            foreach (Control c in panPatientInfo.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.Text = "";
                }
            }
            panPatientInfo.Hide();
            panLogin.Show();
        }

        private void btnToAdmissionForm_Click(object sender, EventArgs e)
        {
            panPatientInfo.Hide();
            panAdmitFrm.Show();
        }

        private void btnToPatientSrch_Click(object sender, EventArgs e)
        {
            panPatientInfo.Hide();
            panSrchnDisChrg.Show();
        }

        private void btnClearPInfo_Click(object sender, EventArgs e)
        {
            foreach (Control c in panPatientInfo.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.Text = "";
                }
            }
        }

        private void btnClearAdmitFrm_Click(object sender, EventArgs e)
        {
            foreach (Control c in panAdmitFrm.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.Text = "";
                }
            }
        }

        private void btnSignoutAF_Click(object sender, EventArgs e)
        {
            foreach (Control c in panAdmitFrm.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.Text = "";
                }
            }
            panAdmitFrm.Hide();
            panLogin.Show();
        }

        private void btnToPatientInfoA_Click(object sender, EventArgs e)
        {
            panAdmitFrm.Hide();
            panPatientInfo.Show();
        }

        private void btnToPatientSrchA_Click(object sender, EventArgs e)
        {
            panAdmitFrm.Hide();
            panSrchnDisChrg.Show();
        }

        private void btnSignoutSrchnDis_Click(object sender, EventArgs e)
        {
            foreach (Control c in panSrchnDisChrg.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
            }
            panSrchnDisChrg.Hide();
            panLogin.Show();
        }

        private void BtnToAdminFrmSD_Click(object sender, EventArgs e)
        {
            panSrchnDisChrg.Hide();
            panAdmitFrm.Show();
        }

        private void btnToPatientInfoSD_Click(object sender, EventArgs e)
        {
            panSrchnDisChrg.Hide();
            panPatientInfo.Show();
        }

        private void btnClearSrchnDis_Click(object sender, EventArgs e)
        {
            foreach (Control c in panSrchnDisChrg.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
            }
        }

        private void btnToBilling_Click(object sender, EventArgs e)
        {
            panPerscriptionsSrch.Hide();
            panBilling.Show();
        }

        private void btnClearPSrch_Click(object sender, EventArgs e)
        {
            foreach (Control c in panPerscriptionsSrch.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
                label43.Text = "";
            }
        }

        private void btnSignOutPS_Click(object sender, EventArgs e)
        {
            foreach (Control c in panPerscriptionsSrch.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                label43.Text = "";
            }
            panPerscriptionsSrch.Hide();
            panLogin.Show();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (Control c in panBilling.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
            }
            label35.Text = "";
        }

        private void btnToPerscriptionSrch_Click(object sender, EventArgs e)
        {
            panBilling.Hide();
            panPerscriptionsSrch.Show();
        }

        private void btnSignoutBilling_Click(object sender, EventArgs e)
        {
            foreach (Control c in panBilling.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
            }
            label35.Text = "";
            panBilling.Hide();
            panLogin.Show();
        }

        private void btnSignOutPExp_Click(object sender, EventArgs e)
        {
            foreach (Control c in panPatientExp.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
                else if (c.GetType() == typeof(ComboBox))
                {
                    c.Text = "";
                    ((ComboBox)c).Items.Clear();
                }
            }
            panPatientExp.Hide();
            panLogin.Show();
        }

        private void btnPaid_Click(object sender, EventArgs e)
        {
            try
            {
                //Retrieve patient info
                string sqlout = $"select * from OutPatients where PatientID={txtPatientIDBillSrch.Text}";

                cmd = new OleDbCommand(sqlout, cs);

                cs.Open();

                cmd.CommandText = sqlout;

                r = cmd.ExecuteReader();

                if (r.Read())
                {
                    ptntID = Convert.ToInt32(r["PatientID"]);
                    emContact = Convert.ToInt32(r["EmergencyContact"]);
                    ptntName = r["PatientName"].ToString();
                    ptntLName = r["PatientLastName"].ToString();
                    emConName = r["EmergencyContactName"].ToString();
                    emConLName = r["EmergencyContactLastName"].ToString();
                    street = r["Street"].ToString();
                    city = r["City"].ToString();
                    country = r["Country"].ToString();
                    diagnosis = r["Diagnosis"].ToString();
                    treatedby = r["Treatedby"].ToString();
                    dateIn = Convert.ToDateTime(r["DateIn"]);
                }
                else
                {
                    MessageBox.Show("No Entries");
                }
                cs.Close();

                //Retrieve expenses info
                string sqlExp = $"select * from Expenses where PatientID={txtPatientIDBillSrch.Text}";
                cmd = new OleDbCommand();

                cs.Open();

                r = cmd.ExecuteReader();

                while (r.Read())
                {
                    //Adds records to a different table to call back on for future visits
                    string sqlInE = $"insert into PreviousMedication(PatientID,PatientName,PatientLastName,Perscription,Amount,Convertion) values('{r["PatientID"]}','{r["PatientName"].ToString()}','{r["PatientLastName"].ToString()}','{r["Item"].ToString()}','{r["Quantity"]}','{r["Convertion"].ToString()}')";

                    cmd.CommandText = sqlInE;
                    cmd.ExecuteNonQuery();
                }
                cs.Close();

                //Adds records to a different table to call back on for future visits
                string sqlPrevOut = $"insert into PreviousPatients(PatientID,PatientName,PatientLastName,EmergencyContact,EmergencyContactName,EmergencyContactLastName,Street,City,Country,Diagnosis,DateIn,Treatedby) values('{ptntID}','{ptntName}','{ptntLName}','{emContact}','{emConName}','{emConLName}','{street}','{city}','{country}','{diagnosis}','{dateIn}','{treatedby}')";

                cmd = new OleDbCommand(sqlPrevOut, cs);

                cs.Open();

                cmd.CommandText = sqlPrevOut;
                cmd.ExecuteNonQuery();

                cs.Close();

                //Removes records of patients after they have paid
                string sqlDIn = $"delete from InPatient where PatientID={txtPatientIDBillSrch.Text}";
                string sqlDOut = $"delete from OutPatients where PatientID={txtPatientIDBillSrch.Text}";
                string sqlDExp = $"delete from Expenses where PatientID={txtPatientIDBillSrch.Text}";

                cmd = new OleDbCommand(sqlDIn, cs);
                OleDbCommand cmd1 = new OleDbCommand(sqlDOut, cs);
                OleDbCommand cmd2 = new OleDbCommand(sqlDExp, cs);

                cs.Open();

                cmd.CommandText = sqlDIn;
                cmd1.CommandText = sqlDOut;
                cmd2.CommandText = sqlDExp;
                cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();

                cs.Close();
                MessageBox.Show("Successful");
            }
            catch(Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }
        
        private void btnPrintScrpt_Click(object sender, EventArgs e)
        {
            if (label43.Text == "")
            {
                //If nothing was searched
                MessageBox.Show("No persciption searched");
            }
            else if (label43.Text == "No perscriptions for this ID")
            {
                //If the search had no results
                MessageBox.Show("No persciption searched");
            }
            else
            {
                //If patient information is showing
                try
                {
                    //Retrieves Perscription information
                    string sqlOut = $"select * from Medication where PatientID={txtPatientIDPersciption.Text}";

                    cmd = new OleDbCommand(sqlOut, cs);

                    cs.Open();

                    r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        //Adds Perscription to differnt table for reference in future visits
                        string sqlIn = $"insert into PreviousMedication(PatientID,PatientName,PatientLastName,Perscription,Amount,Convertion,PerscribedBy,Instructions) values('{r["PatientID"]}','{r["PatientName"].ToString()}','{r["PatientLastName"].ToString()}','{r["Perscription"].ToString()}','{r["Amount"]}','{r["Convertion"].ToString()}','{r["PerscribedBy"].ToString()}','{r["Instructions"].ToString()}')";

                        OleDbCommand cmd1 = new OleDbCommand(sqlIn, cs);

                        cmd1.CommandText = sqlIn;
                        cmd1.ExecuteNonQuery();
                    }

                    //Removes perscription after printing to prevent abusive reprints 
                    string sqlDel = $"delete from Medication where PatientID={txtPatientIDPersciption.Text}";

                    OleDbCommand cmd2 = new OleDbCommand(sqlDel, cs);
                    cmd2.ExecuteNonQuery();

                    cs.Close();

                    //Brings up print screen
                    Print(panPerscriptionsSrch);

                    MessageBox.Show("Perscription Printed");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No Patient ID entered");
                }
                finally
                {
                    cs.Close();
                }
            }
        }

        //Object Variables for printing
        PrintDocument printdoc1 = new PrintDocument();
        PrintPreviewDialog previewdlg = new PrintPreviewDialog();
        Panel pannel = null;

        Bitmap MemoryImage;
        public void GetPrintArea(Panel pnl)
        {
            //Creates print image
            MemoryImage = new Bitmap(pnl.Width, pnl.Height);
            Rectangle rect = new Rectangle(0, 0, pnl.Width, pnl.Height);
            pnl.DrawToBitmap(MemoryImage, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }

        void printdoc1_PrintPage(object sender, PrintPageEventArgs e)
        {
            //Positions image on page
            Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(MemoryImage, (pagearea.Width / 2) - (pannel.Width / 2), pannel.Location.Y);
        }

        public void Print(Panel pnl)
        {
            pannel = pnl;
            GetPrintArea(pnl);

            //To display print screen for printing
            previewdlg.Document = printdoc1;
            previewdlg.ShowDialog();
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            //Brings up print dialog for panel
            Print(panBilling);
        }

        private void btnBackToDiagnose_Click(object sender, EventArgs e)
        {
            panPatientHistory.Hide();
            panDiagnosisnPersriptions.Show();
        }

        private void btnToPatientHistory_Click(object sender, EventArgs e)
        {
            panPatientHistory.Show();
            panDiagnosisnPersriptions.Hide();
        }

        private void btnSrchHistory_Click(object sender, EventArgs e)
        {
            try
            {
                string sql1 = $"select PatientID,PatientName,PatientLastName,EmergencyContact,EmergencyContactName,EmergencyContactLastName,Street,City,Country,Diagnosis,DateIn,Treatedby from PreviousPatients where PatientID={txtPatientIDHistory.Text}";
                string sql2 = $"select PatientID,PatientName,PatientLastName,Perscription,Amount,Convertion,PerscribedBy from PreviousMedication where PatientID={txtPatientIDHistory.Text}";

                gv = new OleDbDataAdapter(sql1, cs);
                OleDbDataAdapter gv1 = new OleDbDataAdapter(sql2, cs);

                cs.Open();

                DataTable data = new DataTable();
                gv.Fill(data);
                dataGridView4.DataSource = data;

                DataTable data1 = new DataTable();
                gv1.Fill(data1);
                dataGridView7.DataSource = data1;

                cs.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("No Patient ID entered");
            }
            finally
            {
                cs.Close();
            }
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            //Loop to clear all controls
            foreach (Control c in panPatientHistory.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)c).Clear();
                }
                else if (c.GetType() == typeof(DataGridView))
                {
                    ((DataGridView)c).DataSource = null;
                }
            }
        }
    }
}
