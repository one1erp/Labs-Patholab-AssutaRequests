using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssutaRequests
{
    public class Patient
    {
        string _gender;
        private DataLayer _dal;
        private string _identity;
        private DateTime? _birthDate;
        private bool _ispassport;
        private string _firstName, _lastName;
       // private long? ClientId;
        string _isPassportStr;


        #region Ctor
        public Patient(MainHeader mh, DataLayer dal)
        {
            _ispassport = mh.IDPatCode == "9";
            _isPassportStr = _ispassport ? "T" : "F";
            _identity = FixIdentity(mh.PatID);
            _firstName = mh.PatFirstName;
            _lastName = mh.PatLastName;
            _gender = GetNautGender(mh.Sex);
            _birthDate = GetDate(mh.BD);
            Program.log(string.Format("{0},{1},{2},{3},{4},{5}",
                _identity, _firstName, _lastName, _gender, _birthDate, _ispassport));

            this._dal = dal;

        }
        #endregion


        public string AddOrEditClient()
        {
            Program.log("Check if client Exists by " + _identity);
            var client = _dal.GetAll<CLIENT>()
                .FirstOrDefault(a => a.NAME == this._identity);


            if (client == null)
            {
                Program.log("Add client");
                var c = _dal.AddClient(_identity, _birthDate, _isPassportStr, _firstName, _lastName, _gender);
                return c.CLIENT_ID.ToString();

            }

            else
            {
                return ApproveExistingClient(client);
            }

            //בדיקות קיום נבדק
            //   אם קיימת רשומת נבדק		Client User	
            //   עם פרמטרים זהים:
            //       1 – Client Type  =  IDPatCode
            //   וגם	2 – u_id_code	  =  PatID
            //   וגם	3 – u_gender    =  Sex 
            //   וגם	4 - U_FIRST_NAME = PatFirstName
            //   וגם	5 - U_LAST_NAME = PatLastName  
            //   ---- >	הנבדק  קיים בבסיס הנתונים
            //   אם לא מתקיים עבור כל 5 הבדיקות
            //       אבל כן מתקיים פרמטרים 	1 + 2 
            //       ---- >	מחייב אישור ידני של קיום הנבדק
            //   אם לא מתקיים 1 + 2
            //       ---- >  נבדק חדש
            //   הקם נבדק חדש בבסיס הנתונים, עם הפרמטרים  1 – 5          
        }

        private string ApproveExistingClient(CLIENT client)
        {
            var cu = client.CLIENT_USER;
            if (RemoveChars(client.NAME) == RemoveChars(_identity)
                && RemoveChars(_firstName) == RemoveChars(cu.U_FIRST_NAME)
                && RemoveChars(_lastName) == RemoveChars(cu.U_LAST_NAME)
                && _gender == cu.U_GENDER)
            {
                Program.log("All client fields are the same");

                return client.CLIENT_ID.ToString();
            }
            else
            {
                string err = "";
                if (RemoveChars(_identity) != RemoveChars(client.NAME))
                    err += ("Name->" + RemoveChars(client.NAME) + "<>" + RemoveChars(_identity) + "; ");
                if (RemoveChars(_firstName) != RemoveChars(cu.U_FIRST_NAME))
                    err += ("First Name ->" + RemoveChars(cu.U_FIRST_NAME) + "<>" + RemoveChars(_firstName) + "; ");
                if (RemoveChars(_lastName) != RemoveChars(cu.U_LAST_NAME))
                    err += ("Last Name ->" + RemoveChars(cu.U_LAST_NAME) + "<>" + RemoveChars(_lastName) + "; ");
                if (cu.U_GENDER == null)
                {
                    err += ("Gender is null; ");
                }
                else if (_gender.Trim() != cu.U_GENDER.Trim())
                    err += ("Gender-> " + cu.U_GENDER.Trim() + "<>" + _gender.Trim() + "; ");
               // if (_isPassportStr.Trim() != cu.U_PASSPORT)
              //      err += ("PASSPORT-> " + cu.U_PASSPORT + "<>" + _isPassportStr.Trim() + ";");


                if (!string.IsNullOrEmpty(err))
                {
                    err = "ON PATIENT " + client.NAME + " " + err;
                    Program.log(err);
                }
                return err;
            }
        }

        private string FixIdentity(string cn)
        {

            if (_ispassport)
            {
                return cn;
            }
            else
            {
                var cn0 = cn;
                while (cn0.Length < 9)
                {
                    cn0 = "0" + cn0;
                }
                return cn0;
            }


        }

        private string GetNautGender(string g)
        {

            switch (g)
            {
                case "1":
                    return "M";
                case "0":
                    return "F";
                default:
                    return "U";

            }
        }

        private DateTime? GetDate(string _birthDate)
        {
            try
            {
                var dttimefull = DateTime.ParseExact(_birthDate, "yyyyMMdd", null);
                Console.WriteLine(dttimefull.ToString());
                return dttimefull;
            }
            catch
            {
                return null;
            }


        }

        private string RemoveChars(string str)
        {
            return str.Trim().Replace("'", "").Replace("  "," ").Replace("-", " ");
        }

    }
}



