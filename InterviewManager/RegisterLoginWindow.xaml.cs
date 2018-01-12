using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using System.Windows.Shapes;
using static InterviewManager.UserWindow;

namespace InterviewManager
{
    /// <summary>
    /// Interaction logic for RegisterLoginWindow.xaml
    /// </summary>
    public partial class RegisterLoginWindow : Window
    {
        private ObservableCollection<SubjectItem> _subjects = null;

        private bool isRegistration = true;
        public bool IsRegistration {
            get
            {
                return isRegistration;
            }
            set
            {
                isRegistration = value;
                if(!value)
                {
                    RegistrationFields.Visibility = Visibility.Hidden;
                    Height = 170;
                }
            }
        }
        public PermissionIDs PermissionID { get; set; } = PermissionIDs.User;

        public enum PermissionIDs
        {
            Admin = 1,
            Interviewer = 2,
            User = 3
        }

        public RegisterLoginWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            _subjects = new ObservableCollection<SubjectItem>();
            _subjects = GetSubjects();
            Specialization.ItemsSource = _subjects;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(IsRegistration)
            {
                Registration();
            }
            else
            {
                LogIn();
            }
        }

        private ObservableCollection<SubjectItem> GetSubjects()
        {
            ObservableCollection<SubjectItem> res = new ObservableCollection<SubjectItem>();
            using (var dal = new DAL())
            {
                using (var rd = dal.Query("SELECT * FROM [Subjects]").ExecuteReader())
                {
                    if(rd.HasRows)
                    {
                        foreach (IDataRecord r in rd)
                        {
                            res.Add(new SubjectItem() { ID = r["ID"].ToString().Trim(), Subject = r["Subject"].ToString().Trim() });
                        }
                    }
                }
            }
            return res;
        }

        private void Registration()
        {
            using (DAL d = new DAL())
            {
                if(string.IsNullOrWhiteSpace(Login.Text) ||
                    string.IsNullOrWhiteSpace(Password.Password) ||
                    string.IsNullOrWhiteSpace(FirstName.Text) ||
                    string.IsNullOrWhiteSpace(LastName.Text) ||
                    string.IsNullOrWhiteSpace(Phone.Text) ||
                    Specialization.SelectedIndex < 0)
                {
                    Error.Content = "Fields can not be empty";
                }
                else
                {
                    if (!d.CheckForExistance("Users", "Login", Login.Text))
                    {
                        if (Password.Password == ConfirmPassword.Password)
                        {
                            using (var v = d.Query("INSERT INTO [Users] ([Login], [Password], [FirstName], [LastName], [Phone], [PermissionID]) VALUES (" +
                                "'" + Login.Text + "', " +
                                "'" + DAL.GetMd5Hash(Password.Password) + "', " +
                                "'" + FirstName.Text + "', " +
                                "'" + LastName.Text + "', " +
                                "'" + Phone.Text + "', " +
                                "'" + (int)PermissionID + "')"
                                ))
                            {
                                using (v.ExecuteReader())
                                {
                                    DialogResult = true;
                                }
                            }
                            using (var v = d.Query("SELECT [ID] FROM [Users] WHERE [Login] = '" + Login.Text + "'").ExecuteReader())
                            {
                                foreach(IDataRecord rcd in v)
                                {
                                    using (DAL dal = new DAL())
                                    {
                                        dal.Query("INSERT INTO [SubjectsUsers] ([SubjectID], [UserID]) VALUES (" + "'" + _subjects[Specialization.SelectedIndex].ID + "', " + "'" + rcd["ID"].ToString() + "')").ExecuteReader().Close();
                                    }
                                }
                            }
                        }
                        else
                        {
                            Error.Content = "Failed to confirm password";
                        }
                    }
                    else
                    {
                        Error.Content = "This login is already exists";
                    }
                }
            }
        }

        private void LogIn()
        {
            using (DAL d = new DAL())
            {
                if (string.IsNullOrWhiteSpace(Login.Text) ||
                    string.IsNullOrWhiteSpace(Password.Password))
                {
                    Error.Content = "Fields can not be empty";
                }
                else
                {
                    if (d.CheckForExistance("Users", "Login", Login.Text))
                    {
                        using (var res = d.Query("SELECT * FROM [Users] WHERE [Login] = '" + Login.Text + "' AND [Password] = '" + DAL.GetMd5Hash(Password.Password) + "'").ExecuteReader())
                        {
                            if (res.HasRows)
                            {
                                foreach (IDataRecord rec in res)
                                {
                                    var uid = rec["ID"].ToString();
                                    using (DAL d2 = new DAL())
                                    {
                                        using (var rdr = d2.Query("SELECT * FROM [Permissions] WHERE [ID] = '" + rec["PermissionID"] + "'").ExecuteReader())
                                        {
                                            foreach (IDataRecord perm in rdr)
                                            {
                                                if (int.Parse(perm["Level"].ToString()) == 0)
                                                {
                                                    Admin(uid);
                                                }
                                                else
                                                {
                                                    if (int.Parse(perm["Level"].ToString()) == 1)
                                                    {
                                                        Interviewer(uid);
                                                    }
                                                    else
                                                    {
                                                        if (int.Parse(perm["Level"].ToString()) == 2)
                                                        {
                                                            User(uid);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Error.Content = "Login failed";
                            }
                        }
                    }
                    else
                    {
                        Error.Content = "This login is not exists";
                    }
                }
            }
        }

        private void Admin(string uid)
        {
            new UserWindow() { Permission = Permissions.Admin, UID = uid }.Show();
            DialogResult = true;
        }

        private void Interviewer(string uid)
        {
            new UserWindow() { Permission = Permissions.Interviewer, UID = uid}.Show();
            DialogResult = true;
        }

        private void User(string uid)
        {
            new UserWindow() { Permission = Permissions.User, UID = uid }.Show();
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
    }

    internal class SubjectItem
    {
        public string ID { get; set; }
        public string Subject { get; set; }

        public override string ToString()
        {
            return Subject;
        }
    }
}
