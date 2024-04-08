using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssutaRequests
{
    public class Arrived2Nautilus
    {
        private Patholab_DAL_V1.DataLayer _dal;
        private PHRASE_HEADER InterfaceParams;
        private U_SAMPLE_MSG_USER SMU;

        string attacedPdfPath = "";

        public Arrived2Nautilus(DataLayer _dal, PHRASE_HEADER InterfaceParams)
        {

            this._dal = _dal;
            this.InterfaceParams = InterfaceParams;
            InterfaceParams.PhraseEntriesDictonary.TryGetValue("Destination Attached Pdf", out attacedPdfPath);
        }

        internal void Run()
        {
            try
            {
                
                Program.log("**************************************\nThird phase:Checks if the messages sent to Instrument,have arrived");
                //Get Message that has SDG
                var messages = from item in _dal.GetAll<U_SAMPLE_MSG_USER>()
                               where item.U_STATUS == "I" && item.U_ORDER_ID.HasValue
                               select item;

                var innerJoinQuery = (from d in _dal.GetAll<SDG_USER>()
                                      join msg in messages on d.U_ORDER_ID equals msg.U_ORDER_ID
                                      where d.SDG.STATUS == "V" || d.SDG.STATUS == "U"
                                      select new
                                      {
                                          d,
                                          msg
                                      }).ToList();
                Program.log(innerJoinQuery.Count() + " New messages on status 'sent to instrument'");
                foreach (var item in innerJoinQuery)
                {
                    SMU = item.msg;
                    item.msg.U_STATUS = "A";
                    Program.log("status changed for " + item.msg.U_SAMPLE_MSG_ID);
                }

                _dal.SaveChanges();
            }
            catch (Exception ex)
            {
                Program.log(SMU != null ? "Error while working on SAMPLE MSG USER number : " + SMU.U_REQUEST_NUM : "Error in Arrived2Nautilus query. ");
                Program.log("THE ERROR IS: " + ex + ". Inner error is : " + ex.InnerException);
            }

        }
    }
}
