using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssutaRequests
{
    public class XML2Nautilus
    {

        private DataLayer _dal;
        private PHRASE_HEADER InterfaceParams;

        const string NEWMSGTYPE = "1";//סוג ממסר חדש
        const string CANCELMSGTYPE = "2";//סוג ממסר לביטול
        const string UPDATEMSGTYPE = "3";//סוג ממסר לעדכון
        string sFrm;


        public XML2Nautilus(DataLayer _dal, PHRASE_HEADER InterfaceParams)
        {
            this._dal = _dal;
            this.InterfaceParams = InterfaceParams;
        }

        internal void Run()
        {
            Program.log("***********************\nFirst phase: reading XML into a Middle table");
            READ_XML();
        }

        private void READ_XML()
        {

            string xmlDir = Program.InputPath;
            var files = Directory.GetFiles(xmlDir, "*.xml");
            Program.log(files.Count() + " XML were found in the directory " + Program.InputPath);
            foreach (var xmlFile in files)
            {
                try
                {
                    Program.log("------------------------- \n Works on " + Path.GetFileNameWithoutExtension(xmlFile));
                    //    if (!HasXMlPdfBoth(xmlFile)) { continue; }

                    string filename = Path.GetFileName(xmlFile);
                    if (!filename.Contains("AS") || filename.Contains("_"))
                    {
                        throw new System.ArgumentException("File does not contain 'AS' or contain '_' in the name");
                    }

                    string xml = File.ReadAllText(xmlFile);


                    Program.log("Converts the XML to an object");
                    var xmlObj = xml.ParseXML<Main>();

                    Program.log("Build SAMPLE MSG entity");
                    var NewIdNbr = InsertRequestMSG(xmlObj);

                    Program.log("NEW SAMPLE MSG ID IS  " + NewIdNbr + "\n"
                    + "Moves XML to backup directory");

                    var newDest = MoveFile(xmlFile);

                    Program.log("Update the middle table in backup directory");
                    UpdateAsSuccess(NewIdNbr, newDest);

                }
                catch (Exception ex)
                {
                    Program.log(xmlFile != null ? "Error while working on a file : " + xmlFile : "Error in Xml2nautilus query. ");
                    Program.log("THE ERROR IS: " + ex + ". Inner error is : " + ex.InnerException);
                }
            }
        }


        long InsertRequestMSG(Main msg)
        {

            var header = msg.Header;

            var id = _dal.GetNewId("SQ_U_SAMPLE_MSG");
            long NewId = Convert.ToInt64(id);
            U_SAMPLE_MSG req = new U_SAMPLE_MSG()
            {
                NAME = NewId.ToString(),
                DESCRIPTION = header.XMLDate + header.XMLHr,
                U_SAMPLE_MSG_ID = NewId,
                VERSION = "1",
                VERSION_STATUS = "A"
            };

            var ed = GetDate(header.ExecuteTime);

            //long? sender = GetAssutaClinic(header.HosCode, header.UnitCode);

            long? sender = GetAssutaClinic(msg.Header.Instname, header.HosCode);

            //מחכה לתשובה מנתנאל

            Patient ptnt = new Patient(header, _dal);
            var clientIdSTR = ptnt.AddOrEditClient();
            var clientId = ToNullableInt(clientIdSTR);
            string errors = "";
            if (clientId == null)
            {
                errors = clientIdSTR;
            }
            var refDocid = GetSupplier(header.RelatedDocID);

            var execDocid = GetSupplier(header.ExecDocID);

            long? partId = GetPart(msg);

            long? customerId = GetCustomer(msg);

            long? secondCustomerId = GetSecondCustomer(msg);

            //PRIORITY
            //conversion from phrase
            var UrgencyPat = GetPriority(msg.Body.Tests.First().UrgencyPat);

            var order = InsertOrder(customerId, secondCustomerId, partId);


            string pdf = "";
            for (int i = 0; i < msg.Header.PDFFiles.Length; i++)
            {
                if (msg.Header.PDFFiles[i] != "")
                {
                    pdf += msg.Header.PDFFiles[i] + ";";
                }
            }

            //לטפל בהודעת ביטול
            //רק אם סטטוס לא V
            string status="";
            switch (header.Status)
            {
                case NEWMSGTYPE:
                    {
                        status = "N";
                        break;
                    }
                case CANCELMSGTYPE:
                    {
                        status = "C";
                        break;
                    }
                case UPDATEMSGTYPE:
                    {
                        U_SAMPLE_MSG_USER s = _dal.FindBy<U_SAMPLE_MSG_USER>(x => x.U_REQUEST_NUM == header.RequestNum.Trim()).FirstOrDefault();
                        if (s == null)
                        {
                            status = "N";
                        }
                        else
                        {
                            status = "U";
                        }
                        break;
                    }
            }

            U_SAMPLE_MSG_USER reqUser = new U_SAMPLE_MSG_USER()
            {
                U_SAMPLE_MSG_ID = NewId,
                U_CASE_FILE = header.CaseFile.Trim(),
                
                U_REQUEST_NUM = header.RequestNum.Trim(),
                U_REQ_NUM_LAB = header.ReqNumLab.Trim(),
                U_SUP_CODE = header.SUPCode.Trim(),
                U_EXECUTE_TIME = ed,
                U_CREATED_ON = DateTime.Now,
                U_PDF = pdf.Trim(),
                U_CLINIC_HOS = header.HosCode,
                U_CLINIC_UNIT = header.UnitCode,


                //Link
                U_CLIENT_ID = clientId,
                U_CLINIC_ID = sender,
                U_IMPLEMENTING_PHYSICIAN = execDocid,
                U_REFERRING_PHYSICIAN = refDocid,
                U_ORDER_ID = order.U_ORDER_ID,
                U_ORDER = order,


                U_PRIORITY = UrgencyPat.Trim(),
                U_INSTNAME = header.Instname.Trim(),

                //Message Type
                U_MESSAGE_TYPE = header.Status.Trim(),
                U_UPDATE_NUMBER = header.UpdateNumber.Trim(),
                U_UPDATE_REASON = header.UpdateReason.Trim(),

                //patient
                U_PATIENT_NAME = header.PatID.Trim(),
                U_PATIENT_FIRST_NAME = header.PatFirstName.Trim(),
                U_PATIENT_LAST_NAME = header.PatLastName.Trim(),
                U_PATIENT_GENDER = header.Sex.Trim(),
                U_PATIENT_BD = header.BD.Trim(),
                U_PATIENT_ID_TYPE = header.IDPatCode.Trim(),

                //Physicians
                U_IMP_PHYSICIAN_NBR = header.ExecDocID.Trim(),
                U_IMP_PHYSICIAN_NAME = header.ExecDocName.Trim(),
                U_REF_PHYSICIAN_NBR = header.RelatedDocID.Trim(),
                U_REF_PHYSICIAN_NAME = header.RelatedDocName.Trim(),


                U_STATUS = status

            };

            errors += _dal.ValidateSampleMsg(reqUser);

            //SDG_USER newSDg = reqUser.U_ORDER.SDG_USER.FirstOrDefault(x => x.SDG.STATUS == "V");
            //errors += AssignContainer(newSDg);//לבדוק 

            if (header.Status == NEWMSGTYPE && !string.IsNullOrEmpty(errors))
            {
                Program.log("=>" + errors);
                reqUser.U_STATUS = "H";
                reqUser.U_ERRORS = errors;
            }


            Program.log("Status of MSG is " + reqUser.U_STATUS);

            //Add to DB
            req.U_SAMPLE_MSG_USER = reqUser;
            _dal.Add(req);
            //  _dal.Add(reqUser);

            //INSERT U_SAMPLE_MSG_ROW
            var tests2add = msg.Body.Tests;

            for (int i = 0; i < tests2add.Length; i++)
            {
                AddRow(tests2add[i], NewId);
            }

            //One transaction for U_SAMPLE_MSG,U_ORDER,U_SAMPLE_MSG_ROW

            _dal.SaveChanges();
            Program.log("Commit Changes");
            return NewId;
        }

        private string GetPriority(string p)
        {
            string key = "PR" + p;

            if (this.InterfaceParams.PhraseEntriesDictonary.ContainsKey(key))
            {
                return this.InterfaceParams.PhraseEntriesDictonary[key];
            }
            return "1";

        }

        private long? GetPart(Main msg)
        {
            var tests2add = msg.Body.Tests;
            var firstTest = tests2add.First();

            long? partId = null;
            string pval = null;

            if (InterfaceParams.PhraseEntriesDictonary.TryGetValue(firstTest.TestCode, out pval))
            {
                partId = ToNullableInt(pval);

            }
            return partId;
        }

        U_ASSUTA_CUSTOMER_USER GetASSUTA_CUSTOMER(string Instname, string hosCode)
        {
            U_ASSUTA_CUSTOMER_USER assutaCustomer = null;
            sFrm = string.Format("Parameters=Instname={0},hosCode={1}", Instname, hosCode);
            Program.log(sFrm);

            var assutaCustomerQuery = _dal.FindBy<U_ASSUTA_CUSTOMER_USER>(x => x.U_ASSUTA_CUSTOMER_NAME == Instname && x.U_ASSUTA_CLINIC_CODE == hosCode);
            if (assutaCustomerQuery != null && assutaCustomerQuery.Count() == 1)
            {
                assutaCustomer = assutaCustomerQuery.FirstOrDefault();

                sFrm = string.Format("U_ASSUTA_CUSTOMER_ID={0},U_ASSUTA_CLINIC={1},U_PATHOLAB_CUSTOMER_CODE={2}, U_PATHOLAB_SECOND_CUSTOMER={3}, U_PRIORITY={4}",
                    assutaCustomer.U_ASSUTA_CUSTOMER_ID, assutaCustomer.U_ASSUTA_CLINIC, assutaCustomer.U_PATHOLAB_CUSTOMER_CODE, assutaCustomer.U_PATHOLAB_SECOND_CUSTOMER, assutaCustomer.U_PRIORITY);
                Program.log(sFrm);

                return assutaCustomer;
            }
            else
            {
                if (assutaCustomerQuery == null)
                    Program.log("No ASSUTA_CUSTOMER");


                if (assutaCustomerQuery.Count() > 1)
                {
                    Program.log("Error  ASSUTA_CUSTOMER assutaCustomerQuery.Count()==2");

                }
                return null;
            }
        }


        private long? GetCustomer(Main msg)
        {

            string instName = msg.Header.Instname;
            string hosCode = msg.Header.HosCode;

            long? customerId = null;

            var assutaCustomer = GetASSUTA_CUSTOMER(instName, hosCode);
            if (assutaCustomer != null)
            {
                customerId = assutaCustomer.U_PATHOLAB_CUSTOMER_CODE;
            }


            return customerId;
        }


        private long? GetSecondCustomer(Main msg)
        {

            string instName = msg.Header.Instname;
            string hosCode = msg.Header.HosCode;
            //  var customerId = 144;//TODO:SELECT FROM DUAL
            long? customerId = null;

            var assutaCustomer = GetASSUTA_CUSTOMER(instName, hosCode);
            if (assutaCustomer != null)
            {
                customerId = assutaCustomer.U_PATHOLAB_SECOND_CUSTOMER;

            }


            else
            {
                var cust = _dal.GetAll<U_CUSTOMER>().FirstOrDefault(x => x.NAME == instName);
                if (cust != null)
                {
                    return cust.U_CUSTOMER_ID;
                }
            }

            return customerId;
        }


        public static long? ToNullableInt(string s)
        {
            long i;
            if (long.TryParse(s, out i))
                return i;
            return null;
        }

        private void AddRow(MainBodyTest mainBodyTest, long ParentId)
        {
            Program.log("Add " + mainBodyTest.SampleBarCode);

            var id = _dal.GetNewId("SQ_U_SAMPLE_MSG_ROW");
            long NewId = Convert.ToInt64(id);

            //Check if sample_msg_row_user already exists
            U_SAMPLE_MSG_ROW_USER samUser = _dal.FindBy<U_SAMPLE_MSG_ROW_USER>(x => x.U_SAMPLE_BARCODE == mainBodyTest.SampleBarCode).FirstOrDefault();
            if (samUser != null)
            {
                Program.log("Sample Msg Row already exsists in the system.");
                return;
            }

            SAMPLE_USER sampleUSer = _dal.FindBy<SAMPLE_USER>(x => x.U_ASSUTA_NUMBER == mainBodyTest.SampleBarCode).FirstOrDefault();
            if (sampleUSer != null)
            {
                Program.log("Sample already exsistes in the system.");
            }

            U_SAMPLE_MSG_ROW req = new U_SAMPLE_MSG_ROW()
            {
                NAME = NewId.ToString(),
                U_SAMPLE_MSG_ROW_ID = NewId,
                VERSION = "1",
                VERSION_STATUS = "A"
            };

            U_SAMPLE_MSG_ROW_USER reqUser = new U_SAMPLE_MSG_ROW_USER()
            {
                U_SAMPLE_MSG_ROW_ID = NewId,
                U_SAMPLE_MSG_ID = ParentId,
                U_SAMPLE_BARCODE = mainBodyTest.SampleBarCode

            };
            req.U_SAMPLE_MSG_ROW_USER = reqUser;
            _dal.Add(req);
            //_dal.Add(reqUser);
            Program.log("Sample Msg Row added with id " + NewId + " and barcode " + reqUser.U_SAMPLE_BARCODE);
        }

        public U_ORDER InsertOrder(long? customerid, long? secondCustomerid, long? partid)
        {
            Program.log("Create Order");
            var id = _dal.GetNewId("SQ_U_ORDER");
            long NewId = Convert.ToInt64(id);


            U_ORDER ord = new U_ORDER()
            {
                NAME = NewId.ToString(),
                U_ORDER_ID = NewId,
                TEMPLATE_ID = 1,//fROM PHRASE
                WORKFLOW_NODE_ID = 281,//fROM PHRASE
                VERSION = "1",
                VERSION_STATUS = "A"
            };

            U_ORDER_USER ordUser = new U_ORDER_USER()
            {
                U_ORDER_ID = NewId,
                U_CREATED_ON = DateTime.Now,
                U_CUSTOMER = customerid,// msg.U_CUSTOMER_ID,
                U_PARTS_ID = partid,// msg.U_PART_ID,
                U_STATUS = "N",
                U_SECOND_CUSTOMER = secondCustomerid


            };

            _dal.Add(ord);
            ord.U_ORDER_USER = ordUser;
            //_dal.Add(ordUser);
            Program.log("Order Created with id " + ord.U_ORDER_ID);
            return ord;
        }



        void UpdateAsSuccess(long NewIdNbr, string newDest)
        {

            U_SAMPLE_MSG_USER msg = _dal.FindBy<U_SAMPLE_MSG_USER>(x => x.U_SAMPLE_MSG_ID == NewIdNbr).FirstOrDefault();
            if (msg != null)
            {
                msg.U_PATH = newDest;
            }
            _dal.SaveChanges();
        }

        private string MoveFile(string xmlFile)
        {

            string NameWithoutExtension = Path.GetFileNameWithoutExtension(xmlFile);
            FileInfo f = new FileInfo(xmlFile);
            var bb = Helpers.GetCreateMyFolder(Program.OuputPath);
            var newDest = Path.Combine(bb, NameWithoutExtension + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");
            File.Move(xmlFile, newDest);
            Program.log("File Moved from " + xmlFile + " to" + newDest);
            return newDest;
        }



        /// <summary>
        /// Get Assuta clinic
        /// </summary>
        /// <param name="site"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public long? GetAssutaClinic(string Instname, string hosCode)
        {

            var assutaCustomer = GetASSUTA_CUSTOMER(Instname, hosCode);


            if (assutaCustomer != null)
            {
                return assutaCustomer.U_ASSUTA_CLINIC;
            }

            return null;
        }


        public long? GetSupplier(string idNbr)
        {
            while (idNbr.Length < 9)
            {
                idNbr = "0" + idNbr;
            }
            var supp = _dal.GetAll<SUPPLIER_USER>().FirstOrDefault(x => x.U_ID_NBR == idNbr);
            if (supp != null)
            {
                return supp.SUPPLIER_ID;

            }
            return null;

        }


        private DateTime? GetDate(string date, string format = "ddMMyyHHmm")
        {
            //0409191656

            try
            {

                var dttimefull = DateTime.ParseExact(date, format, null);

                Program.log(dttimefull.ToString());

                return dttimefull;

            }
            catch (Exception)
            {

                return null;
            }
        }

        //private string AssignContainer(SDG_USER newSDg)
        //{
        //    string errors = "";
        //    try
        //    {

        //        if (newSDg.U_CONTAINER_ID.HasValue)
        //            return "";
        //        //צריך לבדוק שכל הצנצנות של הדרישה נמצאות באותה ציידנית
        //        //לשאול את זיו מה לעשות אם לא כך?
        //        //כרגע אני בודק לפי הצנצנת הראשונה


        //        var samples = newSDg.SDG.SAMPLEs.Select(x => x.EXTERNAL_REFERENCE);
        //        U_CONTAINER_USER cont = null;
        //        var ids = new List<long>();
        //        foreach (var extRef in samples)
        //        {
        //            cont = _dal.FindBy<U_CONTAINER_USER>(x => x.U_REQUESTS.Contains(extRef)).FirstOrDefault();
        //            if (cont == null)
        //            {
        //                return ";Container not found for " + extRef;
        //            }
        //            else if (cont.U_STATUS != "V")
        //            {
        //                return string.Format(";Container {0} is in the {1} ", cont.U_CONTAINER.NAME, cont.U_STATUS);
        //            }
        //            else
        //            {
        //                newSDg.U_CONTAINER_ID = cont.U_CONTAINER_ID;
        //                _dal.SaveChanges();
        //            }

        //        }
        //        return errors;

        //    }
        //    catch (Exception ex)
        //    {

        //        return ex.Message;

        //    }
        //}

    }
}
