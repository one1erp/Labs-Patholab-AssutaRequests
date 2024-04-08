using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssutaRequests
{
    public class ReadyToWork
    {

        private Patholab_DAL_V1.DataLayer _dal;
        private PHRASE_HEADER InterfaceParams;
        SDG_USER newSDg;
        private U_SAMPLE_MSG_USER SMU;

        string attacedPdfPath = "";

        public ReadyToWork(DataLayer _dal, PHRASE_HEADER InterfaceParams)
        {

            this._dal = _dal;
            this.InterfaceParams = InterfaceParams;
            InterfaceParams.PhraseEntriesDictonary.TryGetValue("Destination Attached Pdf", out attacedPdfPath);
        }

        internal void Run()
        {
            try
            {
                Program.log("**************************************\nFourth phase:Checks if the messages with sdg are ready to work.");

                //TODO the query takes too long and it times out, maybe check only sample msg from the last month.

                var newMsg = (from item in _dal.GetAll<U_SAMPLE_MSG_USER>()
                              where (item.U_STATUS == "A" || item.U_STATUS == "P")
                              & item.U_ORDER_ID.HasValue & item.U_ORDER.SDG_USER.Count >= 1
                              select item).ToList();

                Program.log(newMsg.Count() + "SDG's Arrived from Instrument");
                foreach (var item in newMsg)
                {
                    SMU = item;
                    string errors = "";

                    newSDg = null;
                    newSDg = item.U_ORDER.SDG_USER.FirstOrDefault(x => x.SDG.STATUS == "V" || x.SDG.STATUS == "U");
                    if (newSDg == null)
                    {
                        Program.log("No SDG USER for Sample Msg Name: " + item.U_SAMPLE_MSG.NAME);
                        continue;
                    }
                    string sdgName = newSDg.SDG.NAME;

                    errors += GeneratePatrholabNbr(newSDg);

                    errors += GenerateSamplesPatholabNbr(newSDg);

                    errors += AssignContainer(newSDg);//לבדוק 

                    errors += AssignAttachments(item, newSDg);//לבדוק                 

                    if (!string.IsNullOrEmpty(errors))
                    {
                        //TODO check if the status should remain A instead of P
                        item.U_STATUS = "P";
                        Program.log("change status to receive for " + newSDg.SDG.NAME + " to: P");
                    }
                    else
                    {
                        item.U_STATUS = "R";
                        Program.log("change status to receive for " + newSDg.SDG.NAME + " to: R");
                    }


                    item.U_ERRORS = errors;
                    _dal.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                Program.log(SMU != null ? "Error while working on SAMPLE MSG USER number : " + SMU.U_REQUEST_NUM : "Error in ReadyToWork query. ");
                Program.log("THE ERROR IS: " + ex + ". Inner error is : " + ex.InnerException);
            }
        }

        private string GeneratePatrholabNbr(SDG_USER newSDg)
        {

            if (string.IsNullOrEmpty(newSDg.U_PATHOLAB_NUMBER))
            {
                try
                {
                    newSDg.U_PATHOLAB_NUMBER = _dal.GeneratePatholabNumber(newSDg);

                    //TODO:Generate PATHOLAB_NUMBER for sample
                    // 06/10/21 - Or complete the TODO part, made a function name GenerateSamplesPatrholabNbr

                    _dal.SaveChanges();
                }
                catch (Exception)
                {
                    return "Missing Patholab number for " + newSDg.SDG.NAME;
                }
            }

            return null;
            ;
        }

        private string GenerateSamplesPatholabNbr(SDG_USER newSDg)
        {
            try
            {
                if (!string.IsNullOrEmpty(newSDg.U_PATHOLAB_NUMBER))
                {
                    SAMPLE[] samples = newSDg.SDG.SAMPLEs.ToArray();
                    var orderedSamples = samples.OrderBy(sample => sample.SAMPLE_ID);

                    int i = 1;
                    foreach (SAMPLE sample in orderedSamples)
                    {
                        sample.SAMPLE_USER.U_PATHOLAB_SAMPLE_NAME = newSDg.U_PATHOLAB_NUMBER + "." + i;
                        int j = 1;
                        foreach(ALIQUOT aliquot in sample.ALIQUOTs.OrderBy(a => a.ALIQUOT_ID))
                        {
                            aliquot.ALIQUOT_USER.U_PATHOLAB_ALIQUOT_NAME = sample.SAMPLE_USER.U_PATHOLAB_SAMPLE_NAME + "." + j;
                            j++;
                        }
                        i++;
                    }

                    _dal.SaveChanges();
                }
                else
                {
                    return "Unable to generate samples phatolab numbers. Missing Patholab number for " + newSDg.SDG.NAME;
                }

                return null;
            }
            catch (Exception)
            {
                return "Unable to generate samples phatolab numbers. Missing Patholab number for " + newSDg.SDG.NAME;
            }
        }

        private string AssignContainer(SDG_USER newSDg)
        {
            string errors = "";
            try
            {

                if (newSDg.U_CONTAINER_ID.HasValue)
                    return "";
                //צריך לבדוק שכל הצנצנות של הדרישה נמצאות באותה ציידנית
                //לשאול את זיו מה לעשות אם לא כך?
                //כרגע אני בודק לפי הצנצנת הראשונה
                Program.log(string.Format("Trying Assign Container to {0} ", newSDg.SDG.NAME));

                var samples = newSDg.SDG.SAMPLEs.Select(x => x.EXTERNAL_REFERENCE);
                U_CONTAINER_USER cont = null;
                var ids = new List<long>();
                foreach (var extRef in samples)
                {
                    cont = _dal.FindBy<U_CONTAINER_USER>(x => x.U_REQUESTS.Contains(extRef)).FirstOrDefault();
                    if (cont == null)
                    {
                        return ";Container not found for " + extRef;
                    }
                    else if (cont.U_STATUS != "V")
                    {
                        return string.Format(";Container {0} is in the {1} ", cont.U_CONTAINER.NAME, cont.U_STATUS);
                    }
                    else
                    {
                        newSDg.U_CONTAINER_ID = cont.U_CONTAINER_ID;
                        _dal.SaveChanges();
                    }

                }
                return errors;

            }
            catch (Exception ex)
            {

                return ex.Message;

            }
        }

        private string AssignAttachments(U_SAMPLE_MSG_USER item, SDG_USER newSDg)
        {

            string errors = "";
            try
            {

                if (string.IsNullOrEmpty(item.U_PDF))
                    return "";
                if (newSDg.SDG.U_SDG_ATTACHMENT_USER.Count > 0)
                {
                    return "";
                }
                Program.log("Trying Assign Attachments " + newSDg.SDG.NAME);
                string[] files = item.U_PDF.Split(';');
                foreach (var pdf in files)
                {

                    if (!string.IsNullOrEmpty(pdf))
                    {

                        string sourcePath = Path.Combine(Program.InputPath, pdf);

                        if (!File.Exists(sourcePath))
                        {
                            return null; //אסותא ביקשו שאם חסר מסמך שזה לא יעכב את יצירת האובייקט
                            //return ";" + sourcePath + " Not Found";
                        }
                        //Build destination path
                        string destPath = Path.Combine(Helpers.GetCreateMyFolder(attacedPdfPath), pdf);

                        File.Move(sourcePath, destPath);


                        var id = _dal.GetNewId("SQ_U_SDG_ATTACHMENT");
                        long NewId = Convert.ToInt64(id);
                        var name = string.Format("{0}-{1}-{2}", pdf, newSDg.SDG_ID, DateTime.Now);

                        U_SDG_ATTACHMENT newPdf = new U_SDG_ATTACHMENT()
                        {
                            NAME = name,
                            U_SDG_ATTACHMENT_ID = NewId,
                            VERSION = "1",
                            VERSION_STATUS = "A"
                        };

                        U_SDG_ATTACHMENT_USER newPdfUser = new U_SDG_ATTACHMENT_USER()
                        {
                            U_SDG_ID = newSDg.SDG_ID,
                            U_SDG_ATTACHMENT_ID = NewId,
                            U_TITLE = pdf,
                            U_PATH = destPath
                        };
                        newPdf.U_SDG_ATTACHMENT_USER = newPdfUser;
                        _dal.Add(newPdf);
                        //    _dal.Add(newPdfUser);
                        _dal.SaveChanges();

                        Program.log(destPath + " Saved");
                    }
                }
                return errors;
            }
            catch (Exception EAXD)
            {
                Program.log("Error on Assign Attachments " + EAXD.Message);
                return EAXD.Message;
            }
            //TODO:Make a copy
        }
    }
}

//private st