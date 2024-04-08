using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssutaRequests
{
    class Send2Instrument
    {
        private Patholab_DAL_V1.DataLayer _dal;
        private PHRASE_HEADER systemPParams;
        private XML2Nautilus xml;
        private U_SAMPLE_MSG_USER SMU;
        #region Ctor
        public Send2Instrument(DataLayer _dal, PHRASE_HEADER systemPParams)
        {
            this._dal = _dal;
            this.systemPParams = systemPParams;
        }
        #endregion

        public void Run()
        {
            try
            {
                Program.log("***********************\nSecond phase: Create file for instrument.\nGet messages by status  \'new\'");
                //Get New and valid messages
                var newMsg = _dal.GetAll<U_SAMPLE_MSG_USER>().Where(x => x.U_STATUS == "N");
                xml = new XML2Nautilus(_dal, systemPParams);
                Program.log(newMsg.Count() + " New Messages Found");
                U_SAMPLE_MSG_USER[] msgArray = newMsg.ToArray();
                int msgCount = newMsg.Count();
                for (int i = 0; i < msgCount; i++)
                {

                    U_SAMPLE_MSG_USER item = msgArray[i];
                    SMU = item;
                    //מונע מצב שמשתמש מחליף סטטוס למרות שחסרים נתונים
                    Program.log("Double checks if data are missing");
                    string errors = GetErrors(item);
                    errors += checkIfSdgExsists(item);
                    if (string.IsNullOrEmpty(errors))
                    {
                        //Generate file for instrument
                        CreateFile(item);

                        //Change Status
                        Program.log("Change status to I");
                        item.U_STATUS = "I";

                    }
                    else
                    {
                        item = generateCalculatedInfo(item);
                        errors = GetErrors(item);
                        errors += checkIfSdgExsists(item);
                        if (string.IsNullOrEmpty(errors))
                        {
                            //Generate file for instrument
                            CreateFile(item);

                            //Change Status
                            Program.log("Change status to I");
                            item.U_STATUS = "I";

                        }
                        else
                        {
                            Program.log("Change status back to H");
                            item.U_STATUS = "H";
                        }
                    }
                    item.U_ERRORS = errors;

                    //Save Status
                    Program.log("Save Changes");
                    _dal.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Program.log(SMU != null ? "Error while working on SAMPLE MSG USER number : " + SMU.U_REQUEST_NUM : "Error in Send2Instrument query. ");
                Program.log("THE ERROR IS: " + ex + ". Inner error is : " + ex.InnerException);
            }
        }
        /// <summary>
        /// If SDG with the same 'EXTERNAL_REFERENCE' already exsists, do not create another SDG. EXTERNAL_REFERENCE is a unique identifier.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string checkIfSdgExsists(U_SAMPLE_MSG_USER item)
        {
            string requestNum = item.U_REQUEST_NUM;
            SDG sdg = (SDG)_dal.FindBy<SDG>(x => x.EXTERNAL_REFERENCE == requestNum).FirstOrDefault();
            if (sdg != null)
            {
                return "SDG with EXTERNAL_REFERENCE " + sdg.EXTERNAL_REFERENCE + " already exsists";
            }
            return "";
        }

        private U_SAMPLE_MSG_USER generateCalculatedInfo(U_SAMPLE_MSG_USER item)
        {
            item.U_CLINIC_ID = xml.GetAssutaClinic(item.U_CLINIC_HOS, item.U_CLINIC_UNIT);
            item.U_IMPLEMENTING_PHYSICIAN = xml.GetSupplier(item.U_IMP_PHYSICIAN_NBR);
            item.U_REFERRING_PHYSICIAN = xml.GetSupplier(item.U_REF_PHYSICIAN_NBR);
            return item;
        }




        private void CreateFile(U_SAMPLE_MSG_USER item)
        {

            var part = item.U_ORDER.U_ORDER_USER.U_PARTS;
            if (part == null)
            {
                part = _dal.FindBy<U_PARTS>(x => x.U_PARTS_ID == item.U_ORDER.U_ORDER_USER.U_PARTS_ID).SingleOrDefault();

            }

            var sdgLines = GetSDGsection(item, part);
            var sampleLines = GetSampleSection(item, part);
            List<string> lines = new List<string>();
            lines.AddRange(sdgLines);
            lines.AddRange(sampleLines);
            WriteToFile(item, lines);
        }

        private List<string> GetSDGsection(U_SAMPLE_MSG_USER item, U_PARTS part)
        {
            var sdgWf = part.U_PARTS_USER.SDG_WORKFLOW.NAME;

            List<string> lines = new List<string>();
            //Begin Section 
            lines.Add("Begin SDG");//Take from phrase??

            //Fields 
            lines.Add("\"external_ref\",\"Workflow_Name\",\"description\",\"u_patient\",\"u_referring_physician\",\"u_implementing_physician\",\"U_IMPLEMENTING_CLINIC\",\"u_order_id\",\"U_PRIORITY\",\"U_HOSPITAL_NUMBER\",\"U_REQUEST_DATE\"");

            DateTime dt = Convert.ToDateTime(item.U_EXECUTE_TIME);
            
            //Data
            string SDG_data = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\""
                           , item.U_REQUEST_NUM, sdgWf, item.U_SAMPLE_MSG.NAME, item.U_CLIENT_ID, item.U_REFERRING_PHYSICIAN, item.U_IMPLEMENTING_PHYSICIAN, item.U_CLINIC_ID,
                           item.U_ORDER_ID, item.U_PRIORITY, item.U_CASE_FILE, dt.ToString("dd/MM/yyyy HH:mm:ss"));

            Program.log(SDG_data);
            lines.Add(SDG_data);

            //End Section
            lines.Add("End SDG");
            return lines;
        }

        private List<string> GetSampleSection(U_SAMPLE_MSG_USER item, U_PARTS part)
        {

            var rows = item.U_SAMPLE_MSG.U_SAMPLE_MSG_ROW_USER.ToList();

            List<string> lines = new List<string>();


            //Begin Section 
            lines.Add("Begin Sample");//Take from phrase

            //Fields 
            lines.Add("\"external_ref\",\"Workflow_Name\",\"Study_ref\",\"sdg_ref\",\"U_ASSUTA_NUMBER\",\"DESCRIPTION\"");


            string swf = "";
            for (int i = 0; i < rows.Count; i++)
            {
                //Get Sample Workflow
                swf = GetSampleWf(i, part.U_PARTS_USER);

                //Get Sample Name
                var crntsamp = rows[i].U_SAMPLE_BARCODE;

                //Data per row
                string sample_data = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                   crntsamp, swf, "", item.U_REQUEST_NUM, crntsamp, rows[i].U_SAMPLE_MSG_ROW_ID);
                Program.log(sample_data);

                lines.Add(sample_data);
            }
            //End Section
            lines.Add("End Sample");
            return lines;
        }

        private void WriteToFile(U_SAMPLE_MSG_USER item, List<string> lines)
        {
            string newfileName = item.U_REQUEST_NUM + "-" + DateTime.Now.ToString("yyyyMMddHHmmssFFF") + ".zrx";

            //It is best to take from the phrase??            
            string path = Path.Combine(Program.InstrumentInput, newfileName);

            // Create a file to write
            File.WriteAllLines(path, lines);

            Program.log("File Saved in " + path);
        }

        private string GetSampleWf(int i, U_PARTS_USER part)
        {
            if (i > 0)
            {
                return part.SAMPLE_WORKFLOW.NAME;
            }
            else //First Sample
            {
                var ptype = part.U_PART_TYPE;
                if (ptype == "B")
                {
                    return systemPParams.PhraseEntriesDictonary["BIOPSY_FIRST_SAMPLE"];//take from new field from part_user at assuta test
                    //part.U_FIRST_SAMPLE_WORKFLOW.ToString;//take from new field from part_user at assuta test
                }
                else if (ptype == "C")
                {
                    return systemPParams.PhraseEntriesDictonary["CYTOLOGY_FIRST_SAMPLE"];
                }
                else if (ptype == "P")
                {
                    return part.SAMPLE_WORKFLOW.NAME;
                }
                else
                {
                    throw new Exception("ERROR ON FIND SAMPLE WF");
                }
            }
        }

        private string GetErrors(U_SAMPLE_MSG_USER item)
        {

            string errors = _dal.ValidateSampleMsg(item);

            if (!string.IsNullOrEmpty(errors))
                Program.log("=>" + errors);

            return errors;
        }

    }
}
