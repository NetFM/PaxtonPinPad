using System;
using System.Data;
using System.Collections.Generic;
using Paxton.Net2.OemClientLibrary;

namespace Net2_OEM_SDK
{
    class Program
    {
        private const int REMOTE_PORT = 8025;
        private const string REMOTE_HOST = "localhost";
        private OemClient _net2Client = new OemClient(REMOTE_HOST, REMOTE_PORT);
        //public int myDepID;

        static void Main(string[] args)
        {
            if (args[0] != "adduser" && args[0] != "removeuser" && args[0] != "removedepartment")
            {
                Console.WriteLine("Format:  PinUser.exe adduser [department] [First name] [Second name] [email] [from] [until] [code]");
                Console.WriteLine("Example: PinUser.exe adduser visitor_05_16 Joe Bloggs joe@gmail.com  2018-05-01 2018-05-01T07:34:42-5:00 1002");
                Console.WriteLine("Format:  PinUser.exe removeuser [surname] ");
                Console.WriteLine("Example: PinUser.exe removeuser smith");
                Console.WriteLine("Format:  PinUser.exe removedepartment [department]");
                Console.WriteLine("Note:  PinUser.exe removedepartment will remove all users in department]");
                Console.WriteLine("Example: PinUser.exe removedepartment visitor_05_16");
                return;
            }

            Program main = new Program();

            bool res = main.AuthenticateUser("System engineer", "isaac0904");
            if (res != true)
            {
                Console.WriteLine("Incorrect username or password");
                main.Close();
                return;
            }

            Console.WriteLine("Authentication success");

            if (args[0] == "adduser")
            {
                AddUser(main, args);
            } else if (args[0] == "removeuser")
            {
                RemoveUser(main, args);
            }



            main.Close();
            return;
        }

        private static void RemoveUser(Program main, string[] args)
        {
            // Get list of users in department

            var query = "SELECT * from UsersEx";

            var dataSet = main._net2Client.QueryDb(query);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Console.Write("row..." + " ");
                Console.Write(row["Surname"] + "   ");
                Console.WriteLine(row["DepartmentName"]);
                Console.WriteLine(row["UserId"]);




                if (row["Surname"].Equals(args[1]))
                {
                    String userId =  row["UserId"].ToString();
                    Console.Write(userId);

                    String query2 = "DELETE from UsersEx where Surname = 'Pratt'";
                    Console.WriteLine(query2);

                    var dataSet2 = main._net2Client.QueryDb(query2);
                    break;
                }

            }
            
        }

        private static void AddUser(Program main, string[] args)
        {
            int numID;

            IUsers usersSet = main._net2Client.ViewUserRecords(String.Format("Field9_50 = '{0}' AND active=1", args[2]));

            if (usersSet.UsersList().Count > 0)
            {
                Console.WriteLine("xxx Duplicate entry, no thank you!");
                return;
            }

            //public int Day { get; }
            DateTime now = DateTime.Now;

            Console.WriteLine(now.Month);
            Console.WriteLine(now.Day);

            //string depString = "Visitors_"+ now.Month + "_" + now.Day;
            string depString = args[1];

            Console.WriteLine("department = " + depString);

            

            // add new department

            bool ret = main._net2Client.AddDepartment(depString);

            if (ret)
            {
                Console.WriteLine("New department was added " + depString);
            }

            // get new department ID

            IDepartments xxx = main._net2Client.ViewDepartments();

            numID = 0;

            foreach (DepartmentsSet.DepartmentRow dept in xxx.DepartmentsDataSource.Department)
            {
                //Console.WriteLine("here " + dept.Name + dept.DepartmentID);

                if (dept.Name == depString)
                {
                    //Console.WriteLine("gotit" + dept.DepartmentID);
                    numID = dept.DepartmentID;
                    Console.WriteLine("department ID = " + numID + " for " + depString);

                }

            }

            //Console.WriteLine("numID = " + numID);



            int accessLevel = 1; // All hours, all doors
            int departmentId = numID;
            bool antiPassbackInd = false;
            bool alarmUserInd = false;
            string firstName = args[2];
            string middleName = null;
            string surname = args[3];
            string telephoneNo = null;
            string telephoneExtension = null;
            string pinCode = args[7];
            string pictureFileName = null;
            //DateTime activationDate = DateTime.Now;
            DateTime activationDate = DateTime.Parse(args[5]);

            int cardNumber = 0;
            int cardTypeID = 0;
            bool active = true;
            string faxNo = null;
            DateTime expiryDate = DateTime.Parse(args[6]);
            string[] customFields = null;

            Console.WriteLine("New user about to be added " + args[2] + ' ' + args[3] + ' ' + args[7]);


            int userId = main._net2Client.AddNewUser(
                accessLevel, departmentId, antiPassbackInd, alarmUserInd, firstName, middleName, surname,
                telephoneNo, telephoneExtension, pinCode, pictureFileName, activationDate, cardNumber, cardTypeID,
                active, faxNo, expiryDate, customFields
            );

            if (userId == OemClient.ErrorCodes.AddNewUserFailed)
            {
                Console.Write(main._net2Client.LastErrorMessage);
                return;
            }

            Console.WriteLine("New user was added " + args[2] + ' ' + args[7]);
        }


        private bool AuthenticateUser(string userName, string password)
        {
            IOperators operators = _net2Client.GetListOfOperators();
            Dictionary<int, string> operatorsList = operators.UsersDictionary();
            foreach (int userID in operatorsList.Keys)
            {
                if (operatorsList[userID] == userName)
                {
                    Dictionary<string, int> methodList = _net2Client.AuthenticateUser(userID, password);
                    return (methodList != null);
                }
            }
            return false;
        }

        private void Close()
        {
            if (_net2Client != null)
            {
                _net2Client.Dispose();
                _net2Client = null;
            }
        }

       
            
            
    }
}