using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static InterviewManager.RegisterLoginWindow;

namespace InterviewManager
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private ObservableCollection<User> _users = null;

        public enum Permissions
        {
            Admin = 0,
            Interviewer = 1,
            User = 2
        }

        public string UID { get; set; }

        private Permissions permission = Permissions.User;
        public Permissions Permission
        {
            get
            {
                return permission;
            }
            set
            {
                permission = value;
                if (value == Permissions.Admin)
                {
                    MainUI.Children.Remove(UserUI);
                    MainUI.Children.Remove(InterviewerUI);
                    MainUI.Children.Remove(MyDates);
                    Profile.IsEnabled = false;
                }
                if (value == Permissions.Interviewer)
                {
                    MainUI.Children.Remove(AdminUI);
                    MainUI.Children.Remove(UserUI);
                    Profile.IsEnabled = false;
                }
                if (value == Permissions.User)
                {
                    MainUI.Children.Remove(AdminUI);
                    MainUI.Children.Remove(InterviewerUI);
                    ListName.Content = "Interviewers list";
                }
            }
        }

        public UserWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            _users = new ObservableCollection<User>();
            _users = GetUsers();
            UserList.ItemsSource = _users;
            Notifycations();
        }



        private void Notifycations()
        {
            new Thread(() =>
            {
                string ldate = null;
                while(true)
                {
                    try
                    {
                        string date = DateTime.Now.Date.ToShortDateString();

                        if (Permission != Permissions.Admin)
                        {
                            foreach(var v in _users)
                            {
                                foreach(var d1 in GetUserDates(v.ID))
                                {
                                    foreach(var d2 in GetUserDates(UID))
                                    {
                                        if(d1 == d2)
                                        {
                                            if (d1 == DateTime.Now.Date.ToShortDateString())
                                            {
                                                if(ldate != date)
                                                {
                                                    ldate = date;
                                                    MessageBox.Show("You have 1 interview today.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    catch { }
                }
            }){ IsBackground = true }.Start();
        }

        private string GetProfile(string uid)
        {
            string res = "";
            using (DAL d = new DAL())
            {
                using (var v = d.Query("SELECT * FROM [Profiles] WHERE [UserID] = '" + uid + "'").ExecuteReader())
                {
                    foreach(IDataRecord r in v)
                    {
                        res = r["Profile"].ToString().Trim();
                    }
                }
            }
            return res;
        }

        private void SetProfile(string uid, string text)
        {
            using (DAL d = new DAL())
            {
                if(d.CheckForExistance("Profiles", "UserID", uid))
                {
                    d.Query("UPDATE [Profiles] SET [Profile] = '" + text + "' WHERE [UserID] = '" + uid + "'").ExecuteReader().Close();
                }
                else
                {
                    d.Query("INSERT INTO [Profiles] ([UserID], [Profile]) VALUES ('" + uid + "', '" + text + "')").ExecuteReader().Close();
                }
            }
        }

        private string GetSubjectID(string uid)
        {
            string res = "-1";
            using (var dal = new DAL())
            {
                using (var res0 = dal.Query("SELECT [SubjectID] FROM [SubjectsUsers] WHERE [UserID] = '" + uid + "'").ExecuteReader())
                {
                    foreach (IDataRecord rcd in res0)
                    {
                        res = rcd["SubjectID"].ToString();
                    }
                }
            }
            return res;
        }

        private ObservableCollection<User> GetUsers()
        {
            ObservableCollection<User> res = new ObservableCollection<User>();

            using (DAL d = new DAL())
            {
                using (var r = d.Query("SELECT * FROM [Users]").ExecuteReader())
                {
                    foreach(IDataRecord rc in r)
                    {
                        
                        if (rc["PermissionID"].ToString() != ((int)PermissionIDs.Admin).ToString())
                        {
                            if (Permission == Permissions.Admin)
                            {
                                if (rc["PermissionID"].ToString() == ((int)PermissionIDs.Interviewer).ToString())
                                {
                                    res.Add(new User() { ID = rc["ID"].ToString().Trim(), FName = rc["FirstName"].ToString().Trim(), Phone = rc["Phone"].ToString().Trim(), LName = rc["LastName"].ToString().Trim(), IsInterviewer = true });
                                }
                                else
                                {
                                    res.Add(new User() { ID = rc["ID"].ToString().Trim(), FName = rc["FirstName"].ToString().Trim(), LName = rc["LastName"].ToString().Trim(), Phone = rc["Phone"].ToString().Trim() });
                                }
                            }

                            if (Permission == Permissions.Interviewer)
                            {
                                if (rc["PermissionID"].ToString() != ((int)PermissionIDs.Interviewer).ToString())
                                {
                                    if(GetSubjectID(rc["ID"].ToString()) == GetSubjectID(UID))
                                    {
                                        res.Add(new User() { ID = rc["ID"].ToString().Trim(), FName = rc["FirstName"].ToString().Trim(), LName = rc["LastName"].ToString().Trim(), Phone = rc["Phone"].ToString().Trim() });
                                    }
                                }
                            }

                            if (Permission == Permissions.User)
                            {
                                if (rc["PermissionID"].ToString() == ((int)PermissionIDs.Interviewer).ToString())
                                {
                                    if (GetSubjectID(rc["ID"].ToString()) == GetSubjectID(UID))
                                    {
                                        res.Add(new User() { ID = rc["ID"].ToString().Trim(), FName = rc["FirstName"].ToString().Trim(), LName = rc["LastName"].ToString().Trim(), IsInterviewer = true, Phone = rc["Phone"].ToString().Trim() });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        private List<string> GetUserDates(string ID)
        {
            List<string> dates = new List<string>();
            string q = "SELECT [Date] FROM [InterviewManager].[dbo].[InterviewDates] " +
            "WHERE [ID] IN " +
            "(SELECT [InterviewDatesID] FROM [InterviewManager].[dbo].[InterviewDatesUsers] " +
            "WHERE [UserID] = '" + ID + "')";

            using (DAL d = new DAL())
            {
                using (var v = d.Query(q).ExecuteReader())
                {
                    foreach (IDataRecord r in v)
                    {
                        if (v.HasRows)
                        {
                            var date = v["Date"].ToString();
                            if (DateTime.Parse(date) >= DateTime.Now.Date.Date)
                            {
                                dates.Add(date.Trim());
                            }
                        }
                    }
                }
            }
            return dates;
        }

        private List<string> GetUserDates(string ID, string iid)
        {
            List<string> dates = new List<string>();
            string q = "SELECT [Date] FROM [InterviewDates] " +
            "WHERE [ID] IN " +
            "(SELECT [InterviewDatesID] FROM [InterviewDatesUsers] " +
            "WHERE [UserID] = '" + ID + "')";

            using (DAL d = new DAL())
            {
                using (var v = d.Query(q).ExecuteReader())
                {
                    foreach (IDataRecord r in v)
                    {
                        if (v.HasRows)
                        {
                            var date = v["Date"].ToString();
                            if (DateTime.Parse(date) >= DateTime.Now.Date)
                            {
                                dates.Add(date.Trim());
                            }
                        }
                    }
                }
            }

            q = "SELECT [Date] FROM [InterviewDates] " +
            "WHERE [ID] IN " +
            "(SELECT [InterviewDatesID] FROM [InterviewDatesUsers] " +
            "WHERE [UserID] = '" + iid + "')";

            List<string> dates2 = new List<string>();

            using (DAL d = new DAL())
            {
                using (var v = d.Query(q).ExecuteReader())
                {
                    foreach (IDataRecord r in v)
                    {
                        if (v.HasRows)
                        {
                            var date = v["Date"].ToString();
                            if (DateTime.Parse(date) >= DateTime.Now.Date)
                            {
                                dates2.Add(date.Trim());
                            }
                        }
                    }
                }
            }

            List<string> datesRes = new List<string>();

            foreach(var v in dates2)
            {
                if(dates.Contains(v))
                {
                    datesRes.Add(v);
                }
            }

            return datesRes;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void AddInterviewer_Click(object sender, RoutedEventArgs e)
        {
            if(new RegisterLoginWindow() { PermissionID = PermissionIDs.Interviewer }.ShowDialog().Value)
            {
                _users.Clear();
                foreach(var v in GetUsers())
                {
                    _users.Add(v);
                }
            }
        }

        private void RemoveInterviewer_Click(object sender, RoutedEventArgs e)
        {
            var i = UserList.SelectedIndex;
            if (i > -1)
            {
                if(_users[i].IsInterviewer)
                {
                    using (DAL d = new DAL())
                    {
                        d.Query("DELETE FROM [Users] WHERE [ID] = '" + _users[i].ID + "'").ExecuteReader().Close();
                        _users.Clear();
                        foreach (var v in GetUsers())
                        {
                            _users.Add(v);
                        }
                    }
                }
            }
        }

        private void SelectionChanged()
        {
            Calendar.SelectedDates.Clear();
            var i = UserList.SelectedIndex;
            if (i != -1)
            {
                PhoneL.Content = "Contact data: " + _users[i].Phone;
                if (Permission == Permissions.Admin || Permission == Permissions.Interviewer)
                {
                    if (_users[i].IsInterviewer)
                    {
                        Profile.IsEnabled = false;
                        foreach (string v in GetUserDates(_users[i].ID))
                        {
                            Calendar.SelectedDates.Add(DateTime.Parse(v));
                        }
                    }
                    else
                    {
                        Profile.IsEnabled = true;
                        foreach (string v in GetUserDates(_users[i].ID))
                        {
                            Calendar.SelectedDates.Add(DateTime.Parse(v));
                        }
                    }
                }
                else
                {
                    foreach (string v in GetUserDates(_users[i].ID))
                    {
                        Calendar.SelectedDates.Add(DateTime.Parse(v));
                    }
                }
            }
        }

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            if(Permission == Permissions.User)
            {
                var w = new ProfileWindow() { ProfileIn = GetProfile(UID) };
                if(w.ShowDialog().Value)
                {
                    SetProfile(UID, w.ProfileOut);
                }
            }
            else
            {
                new ProfileWindow() { ProfileIn = GetProfile(_users[UserList.SelectedIndex].ID), IsUser = false}.ShowDialog();
            }
        }

        private void InsertDate(DateTime date)
        {
            using (DAL d = new DAL())
            {
                d.Query("INSERT INTO [InterviewDates] ([Date]) VALUES ('" + date.ToShortDateString() + "')").ExecuteReader().Close();
                string lid = "";
                using (var v = d.Query("SELECT TOP 1 ID FROM [InterviewDates] ORDER BY ID DESC").ExecuteReader())
                {
                    foreach (IDataRecord r in v)
                    {
                        lid = r["ID"].ToString();
                    }
                }
                d.Query("INSERT INTO [InterviewDatesUsers] ([InterviewDatesID], [UserID]) VALUES ('" + lid + "', '" + UID + "')").ExecuteReader().Close();
            }
        }

        private void DeleteDate(DateTime date)
        {
            using (DAL d = new DAL())
            {
                List<string> ids = new List<string>();
                using (var v = d.Query("SELECT [ID] FROM [InterviewDates] WHERE [Date] = '" + date.ToShortDateString() + "'").ExecuteReader())
                {
                    foreach (IDataRecord r in v)
                    {
                        ids.Add(r["ID"].ToString());
                    }
                }

                List<Tuple<string, string>> ids2 = new List<Tuple<string, string>>();
                using (var v = d.Query("SELECT * FROM [InterviewDatesUsers]").ExecuteReader())
                {
                    foreach(IDataRecord r in v)
                    {
                        if (ids.Contains(r["InterviewDatesID"].ToString()))
                        {
                            ids2.Add(new Tuple<string, string>(r["ID"].ToString(), r["UserID"].ToString()));
                        }
                        else
                        {
                            ids.Remove(r["InterviewDatesID"].ToString());
                        }
                    }
                }

                if(Permission == Permissions.Interviewer)
                {
                    foreach (var v in ids2)
                    {
                        d.Query("DELETE FROM [InterviewDatesUsers] WHERE [ID] = '" + v.Item1 + "'").ExecuteReader().Close();
                    }

                    foreach (var v in ids)
                    {
                        d.Query("DELETE FROM [InterviewDates] WHERE [ID] = '" + v + "'").ExecuteReader().Close();
                    }
                }
                if(Permission == Permissions.User)
                {
                    foreach (var v in ids2)
                    {
                        if(UID == v.Item2)
                        {
                            d.Query("DELETE FROM [InterviewDatesUsers] WHERE [ID] = '" + v.Item1 + "'").ExecuteReader().Close();
                        }
                    }
                }
            }
        }

        private void MyDates_Click(object sender, RoutedEventArgs e)
        {
            Calendar.SelectedDates.Clear();
            if (permission == Permissions.Interviewer)
            {
                foreach (string v in GetUserDates(UID))
                {
                    Calendar.SelectedDates.Add(DateTime.Parse(v));
                }
            }
            else
            {
                var i = UserList.SelectedIndex;
                if (i > -1)
                {
                    foreach (string v in GetUserDates(UID, _users[i].ID))
                    {
                        Calendar.SelectedDates.Add(DateTime.Parse(v));
                    }
                }
            }
        }

        private void AddDate_Click(object sender, RoutedEventArgs e)
        {
            if (Calendar.SelectedDates.Count == 1)
            {
                if (((DateTime)Calendar.SelectedDate) >= DateTime.Now.Date)
                {
                    List<string> dates = GetUserDates(UID);
                    bool ex = false;
                    foreach (var v in dates)
                    {
                        if (((DateTime)Calendar.SelectedDate) == DateTime.Parse(v))
                        {
                            ex = true;
                        }
                    }
                    if (!ex)
                    {
                        InsertDate((DateTime)Calendar.SelectedDate);
                    }
                }
            }
        }

        private void RemoveDate_Click(object sender, RoutedEventArgs e)
        {
            if (Calendar.SelectedDates.Count == 1)
            {
                if (((DateTime)Calendar.SelectedDate) >= DateTime.Now.Date)
                {
                    DeleteDate((DateTime)Calendar.SelectedDate);
                }
            }
        }

        private void SubscribeToInterview_Click(object sender, RoutedEventArgs e)
        {
            int i = UserList.SelectedIndex;
            if (i > -1)
            {
                if (Calendar.SelectedDates.Count == 1)
                {
                    if (((DateTime)Calendar.SelectedDate) >= DateTime.Now.Date)
                    {
                        List<string> dates = GetUserDates(UID);
                        bool ex = false;
                        foreach (var v in dates)
                        {
                            if (((DateTime)Calendar.SelectedDate) == DateTime.Parse(v))
                            {
                                ex = true;
                            }
                        }
                        if (!ex)
                        {
                            foreach (var v in GetUserDates(_users[i].ID))
                            {
                                if (v == ((DateTime)Calendar.SelectedDate).ToShortDateString())
                                {
                                    InsertDate((DateTime)Calendar.SelectedDate);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UnsubscribeFromInterview_Click(object sender, RoutedEventArgs e)
        {
            if (Calendar.SelectedDates.Count == 1)
            {
                if (((DateTime)Calendar.SelectedDate) >= DateTime.Now.Date)
                {
                    DeleteDate((DateTime)Calendar.SelectedDate);
                }
            }
        }

        private void UserList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SelectionChanged();
        }
    }


    public class User
    {
        public string ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Phone { get; set; }
        private bool isInterviewer = false;
        public bool IsInterviewer
        {
            get
            {
                return isInterviewer;
            }
            set
            {
                isInterviewer = value;
                if(value)
                {
                    DotVisibility = Visibility.Visible;
                }
            }
        }

        public Visibility DotVisibility { get; set; } = Visibility.Hidden;

        public override string ToString()
        {
            return FName + " " + LName;
        }
    }
}
