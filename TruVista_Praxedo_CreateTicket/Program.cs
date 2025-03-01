using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TruVista_Praxedo_CreateTicket.BEM;

namespace TruVista_Praxedo_CreateTicket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //pull datat from ERP/CRM
            //Build ticket/event using praxedo soap object service reference from data pulled from truVista
            //send the event e.i ticket request to Praxedo capture response
            //return response to truvista
        }
        public static businessEvent BuildTicket(DataRow ticketData, Data ticket,  RunTimeConfigs config)
        {


            string[] PROPERTY = new string[5];
            int priority_lv = 0;
            var CoreData = new coreData
            {
                contacts = null,
                referentialData = null,
                description = ticket.Description,
                priority = 0,
                creationDateSpecified = true,
                expirationDateSpecified = true,
                //expirationDate = runTime.Do_Before,
                earliestDateSpecified = true,
                //earliestDate = runTime.Do_After,
                //creationDate = runTime.CREATIONDATE,
            };

            var QD_Type = new businessEventType
            {
                id = ticket.WorkOrderCode,//workorder type
                //duration = 60,
                skills = null,
                extensions = null
            };
            var QD = new qualificationData
            {
                //expectedItems = items,
                instructions = null,
                type = QD_Type
            };
            var contract = new contractData()
            {
                extensions = null,//?
                id = null//region @manduco
                //remove this for test env
            };///REMOVE
            DateTime time = DateTime.Now;
            DateTime dt = DateTime.Now;
            dt.ToString("h:mm tt");

            string msg = null;
            if (config.Debug_mode)
            {
                msg = "TEST WORKORDER";
            }


            var Event = new businessEvent
            {
                completionData = null,
                id = ticket.WorkOrder,
                coreData = CoreData,
                qualificationData = QD,
                contractData = contract,
                message = msg,
                status = status.QUALIFIED,
                statusSpecified = true,
                attachments = null
            };

            Console.WriteLine("    ticket data prepared");
            ticket.Clear();//ckears all the varrs just to make sure 


            return Event;
        }
        public static void SendReqest_ToPraxedo(BusinessEventManagerClient client, businessEvent[] events,  Data ticket, RunTimeConfigs config)
        {

            //if (events[0] != null){

            try
            {
                Console.WriteLine("---------------");
                client.Open();
                OperationContextScope scope = new OperationContextScope(client.InnerChannel);
                var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));
                OperationContext.Current.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;


                var result = client.createEvents(events, new BEM.wsEntry[1]);// Performs the WS call
                int pos = 0;
                int ticktNumber = pos + 1;

                int countofticketsent = result.results.Count();

                foreach (var eventResult in result.results)
                {
                    string WO = eventResult.id.ToString();

                        if (eventResult.resultCode.Equals(0))
                        {
                            //

                            Console.WriteLine("        !update made in slx\n");
                        }
                        else if (eventResult.resultCode.Equals(102))
                        {
                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            Console.WriteLine("Duplicate already in praxedo: ");
                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }
                        else if (eventResult.resultCode.Equals(113))
                        {

                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            Console.WriteLine("workourder type not in praxedo: ");
                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                        }
                        else
                        {
                            string e = result.results[0].message ?? "did not update";
                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            Console.WriteLine(e);
                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }
                        //Console.WriteLine(" ");
                        pos++;
                        ticktNumber++;
                }

            }
            finally
            {
                client.Close();
            }
            //}
            //else{
            //    Console.WriteLine("No ticket to send");
            //}
        }
    }

    public class RunTimeConfigs
    {

        public bool Debug_mode { get; set; } = false;
        public string Prodconnstr { get; set; } = @"";
        public string Debugconnstr { get; set; } = @"";
        public bool use_PraxedoEvalEnv { get; set; } = false;
        public bool use_PraxedoProdEnv { get; set; } = false;
        public bool use_Prod_CRMERP { get; set; } = false;
        public bool use_Eval_CRMERP { get; set; } = false;
        public string Praxedo_prod_api_username { get; set; } = "";
        public string Praxedo_prod_api_pass { get; set; } = "";
        public string Praxedo_EvalEnv_api_username { get; set; } = "";
        public string Praxedo_EvalEnv_api_pass { get; set; } = "DrfPraxedo2024!";

        //public string connstr
        //{
        //    get
        //    {
        //        //if (Debug_mode)
        //        //{
        //        //    return Debugconnstr;
        //        //}
        //        //else if (use_Prod_CRMERP)
        //        //{
        //        //    return Prodconnstr;
        //        //}
        //        //else
        //        //{
        //        //    return Prodconnstr;
        //        //}
        //    }
        //}
        public (string username, string pass) PraxedoEnv
        {
            get
            {
                if (use_PraxedoEvalEnv)
                {
                    return (Praxedo_EvalEnv_api_username, Praxedo_EvalEnv_api_pass);
                }
                else
                {
                    return (Praxedo_prod_api_username, Praxedo_prod_api_pass);
                }
            }
        }
    }
    public class Data
    {
        public string PraxedoId { get; set; }
        public string WorkOrder { get; set; }
        public string WorkOrderNumber { get; set; }
        public string WorkOrderType { get; set; }
        public string TicketId { get; set; }
        public string LeadNumber { get; set; }
        public string CreateDate { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Description { get; set; }
        public string WorkOrderCode { get; set; }
        public string Duration { get; set; }
        public string Instruction { get; set; }
        public string ScheduledDate { get; set; }
        public string AssignedToId { get; set; }

        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Mobile { get; set; }
        public string StoreAccountId { get; set; }
        public string StoreName { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
        public string StoreCity { get; set; }
        public string StoreState { get; set; }
        public string StorePostalCode { get; set; }
        public string StoreNumber { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public void Clear()
        {
            PraxedoId = null;
            WorkOrder = null;
            WorkOrderNumber = null;
            TicketId = null;
            LeadNumber = null;
            CreateDate = null;
            PostalCode = null;
            FirstName = null;
            Description = null;
            WorkOrderCode = null;
            Duration = null;
            Instruction = null;
            ScheduledDate = null;
            AssignedToId = null;
            Email = null;
            HomePhone = null;
            WorkPhone = null;
            Mobile = null;
            State = null;
            Address1 = null;
            Address2 = null;
            City = null;

        }
        public void GenerateFakeData()
        {
            ///

        }

    }
}
