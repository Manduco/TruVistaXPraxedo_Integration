using Bogus;
using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TruVista_Praxedo_CreateTicket.BEM;
using DataSet = System.Data.DataSet;

namespace TruVista_Praxedo_CreateTicket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var client = new BusinessEventManagerClient();
            RunTimeConfigs configs = new RunTimeConfigs();//custom class for configs setting
            Data data = new Data();//custom class for configs setting
            configs.Debug_mode = true;
            configs.use_fakedata = true;
            configs.use_PraxedoEvalEnv = true;


            if (configs.use_fakedata)
            {
                data.GenerateFakeData();//for testing and debiggin
            }
            else
            {
                ///productions
                //GetTickets();//get workorders from truvista 
            }


            Console.WriteLine($"Fake Name: {data.FirstName} {data.LastName}");
            Console.WriteLine($"Fake Email: {data.Email}");
            Console.WriteLine($"Fake Work Order: {data.WorkOrder}");
            // businessEvent ticket = BuildTicket(data, configs);//not done

            businessEvent[] tickets = new businessEvent[1];
            businessEvent ticket = BuildTicket(data, configs);
                          tickets[0] = ticket; 

            if (ticket != null){
                SendReqest_ToPraxedo(tickets, data, configs);
            }
            //
            //Submission
            //For this step, submit the URL for the github project repository and a comment indicating 'ready for review.' Within the repository make a folder called prototype and a(separate) readme, which includes a narrative with screenshot images or links to HTML documents outlining the functionality of the prototype.
            //Sketches or mockups are acceptable at this stage.If you are comfortable creating HTML code at this point, it will be a helpful headstart for later.
            //    It's important to contain the prototype in a separate (or sub-) folder to clearly identify this as a prototype with no expetation for finished functionality.

            Console.WriteLine($"Fake Email: {data.Email}");
            //SendReqest_ToPraxedo(ticket, data, configs);
            //pull data from ERP/CRM
            //Build ticket/event using praxedo soap object service reference from data pulled from truVista
            //send the event e.i ticket request to Praxedo capture response
            //return response to truvista
        }
        public static DataTable GetTickets(RunTimeConfigs config, int amount)
        {
            // this function pulls real data from a production database not complete
            string connstr = config.connstr;
            if (amount <= 0) { amount = 1; }

            OleDbConnection conn = new OleDbConnection(connstr);
            DataTable customTable = new DataTable();

            string SqL = string.Format(@"select top {0} * from sysdba.PRAXEDO_TICKETS order by CREATEDATE desc", amount);
            //string SqL = string.Format(@"select * from sysdba.PRAXEDO_TICKETS where workorder like '%{0}%'", workOrderNumber);

            try
            {
                using (OleDbCommand command = new OleDbCommand(SqL, conn))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                    {
                        try
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet, "tickets");
                            DataTable dataTable = dataSet.Tables["tickets"];

                            ////Console.WriteLine("  !Found Workorder in DB:" + workOrderNumber);
                            return dataTable;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            throw;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Log the error or take appropriate action
                // Return the custom table or any other suitable response
                return customTable;
            }

        }
        public static businessEvent BuildTicket(Data ticket,  RunTimeConfigs config)
        {

            //string[] PROPERTY = new string[5];
            //int priority_lv = 0;
            contact[] contacts = Build_Contacts(ticket);
            var CoreData = new coreData
            {
                contacts = contacts,
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
            Console.WriteLine("check");
            var QD_Type = new businessEventType
            {
                id = ticket.WorkOrderCode,//workorder type
                //duration = 60,
                skills = null,
                extensions = null
            };
            Console.WriteLine("check2");
            var QD = new qualificationData
            {
                //expectedItems = items,
                instructions = null,
                type = QD_Type
            };
            Console.WriteLine("check");
            var contract = new contractData()
            {
                extensions = null,//?
                id = null//region @manduco
                //remove this for test env
            };
            Console.WriteLine("check");
            //DateTime time = DateTime.Now;
            //DateTime dt = DateTime.Now;
            //dt.ToString("h:mm tt");

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
        public static contact[] Build_Contacts(Data ticket)
        {
            int numberOfContacts = 0;
            var HomePhone = new contact();
            var workPhone = new contact();
            var Mobile = new contact();
            var Email = new contact();
            int index = 0;

            string HOMEPHONE = ticket.HomePhone;
            string WORKPHONE = ticket.WorkPhone;
            string MOBILE = ticket.Mobile;
            string EMAIL = ticket.Email;


            if (string.IsNullOrEmpty(EMAIL))
            {
                EMAIL = "NOT-PROVIDED";
            }

            HOMEPHONE = Format_PhoneNumber(HOMEPHONE);
            WORKPHONE = Format_PhoneNumber(WORKPHONE);
            MOBILE = Format_PhoneNumber(MOBILE);

            if (!string.IsNullOrEmpty(HOMEPHONE)) { numberOfContacts++; }
            if (!string.IsNullOrEmpty(WORKPHONE)) { numberOfContacts++; }
            if (!string.IsNullOrEmpty(MOBILE)) { numberOfContacts++; }
            if (!string.IsNullOrEmpty(EMAIL)) { numberOfContacts++; }

            contact[] contacts = new contact[numberOfContacts];

            if (!string.IsNullOrEmpty(HOMEPHONE))
            {
                HomePhone = new contact
                {
                    label = "Home Phone",
                    flags = 1,
                    type = type.PHONE,
                    typeSpecified = true,
                    extensions = null,
                    coordinates = HOMEPHONE,

                };
                contacts[index++] = HomePhone;
            }
            if (!string.IsNullOrEmpty(WORKPHONE))
            {

                workPhone = new contact
                {
                    label = "WorkPhone",
                    flags = 1,
                    type = type.PHONE,
                    typeSpecified = true,
                    extensions = null,
                    coordinates = WORKPHONE,

                };
                contacts[index++] = workPhone;
            }
            if (!string.IsNullOrEmpty(MOBILE))
            {

                Mobile = new contact
                {
                    label = "Mobile",
                    flags = 1,
                    type = type.MOBILE,
                    typeSpecified = true,
                    extensions = null,
                    coordinates = MOBILE,

                };
                contacts[index++] = Mobile;
            }
            if (!string.IsNullOrEmpty(EMAIL))
            {

                var EMAILC = new contact
                {
                    label = "Email",
                    flags = 1,
                    type = type.EMAIL,
                    typeSpecified = true,
                    extensions = null,
                    coordinates = EMAIL,

                };
                contacts[index++] = EMAILC;
            }

            return contacts;
        }
        public static void SendReqest_ToPraxedo(businessEvent[] events,  Data ticket, RunTimeConfigs config)
        {
            var client = new BusinessEventManagerClient();
            client.ClientCredentials.UserName.UserName = config.PraxedoEnv.username;
            client.ClientCredentials.UserName.Password = config.PraxedoEnv.pass;
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
        public static string Format_PhoneNumber(string phoneNumber)
        {
            // Assuming the input is always 10 digits
            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber != " ")
            {
                return string.Format("({0}) {1}-{2}",
                phoneNumber.Substring(0, 3),
                phoneNumber.Substring(3, 3),
                phoneNumber.Substring(6, 4));

            }
            else if (phoneNumber == " ")
            {
                return "";
            }
            else
            {
                return null;
            }
        }
    }

    public class RunTimeConfigs
    {

        public bool Debug_mode { get; set; } = false;
        public bool use_fakedata { get; set; } = false;
        public string Prodconnstr { get; set; } = @"";
        public string Debugconnstr { get; set; } = @"";
        public bool use_PraxedoEvalEnv { get; set; } = false;
        public bool use_PraxedoProdEnv { get; set; } = false;
        public bool use_Prod_CRMERP { get; set; } = false;
        public bool use_Eval_CRMERP { get; set; } = false;
        public string Praxedo_prod_api_username { get; set; } = "";
        public string Praxedo_prod_api_pass { get; set; } = "";
        public string Praxedo_EvalEnv_api_username { get; set; } = "truvista.webservice";
        public string Praxedo_EvalEnv_api_pass { get; set; } = "truvistaPraxedo01!";

        //RFisherProfilingSolutions
        //truvista.api
        //truvistaPraxedo01!
        public string connstr
        {
            get
            {
                if (Debug_mode)
                {
                    return Debugconnstr;
                }
                else if (use_Prod_CRMERP)
                {
                    return Prodconnstr;
                }
                else
                {
                    return Prodconnstr;
                }
            }
        }
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
            var faker = new Faker();

            PraxedoId = faker.Random.Guid().ToString();
            WorkOrder = faker.Random.AlphaNumeric(10);
            WorkOrderNumber = faker.Random.Number(100000, 999999).ToString();
            WorkOrderType = faker.PickRandom(new[] { "Repair", "Installation", "Maintenance" });
            TicketId = faker.Random.AlphaNumeric(8);
            LeadNumber = faker.Random.AlphaNumeric(6);
            CreateDate = faker.Date.Past(1).ToString("yyyy-MM-dd HH:mm:ss");

            FirstName = "Armando";//faker.Name.FirstName();
            LastName = "Zincke"; faker.Name.LastName();

            Description = faker.Lorem.Sentence();
            WorkOrderCode = faker.Random.AlphaNumeric(6);
            Duration = faker.Random.Number(1, 8).ToString() + " hours";
            Instruction = faker.Lorem.Sentence();
            ScheduledDate = faker.Date.Future().ToString("yyyy-MM-dd HH:mm:ss");
            AssignedToId = faker.Random.Guid().ToString();

            Email = faker.Internet.Email();
            HomePhone = "3058781599";//faker.Phone.PhoneNumber(); //only 3058781599 format
            WorkPhone = faker.Phone.PhoneNumber("##########");
            Mobile = null;//faker.Phone.PhoneNumber();

            StoreAccountId = faker.Random.Guid().ToString();
            StoreName = faker.Company.CompanyName();
            StorePhone = faker.Phone.PhoneNumber();
            StoreAddress = faker.Address.StreetAddress();
            StoreCity = faker.Address.City();
            StoreState = faker.Address.StateAbbr();
            StorePostalCode = faker.Address.ZipCode();
            StoreNumber = faker.Random.Number(1, 9999).ToString();

            Address1 = faker.Address.StreetAddress();
            Address2 = faker.Address.SecondaryAddress();
            City = faker.Address.City();
            State = faker.Address.StateAbbr();
            PostalCode = faker.Address.ZipCode();
        }

    }
}
