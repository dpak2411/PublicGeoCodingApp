using System;
using System.Collections.Generic;
using AlteryxGalleryAPIWrapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace PublicGeoCodingApp
{
    [Binding]
    public class PublicGeoCodingAppSteps
    {

        private string alteryxurl;
        private string _sessionid;
        private string _appid;
        private string _userid;
        private string _appName;
        private string jobid;
        private string outputid;
        private string validationId;
        private string _appActualName;
        private int messagecount;
        private dynamic statusresp;

        private Client Obj = new Client("https://gallery.alteryx.com/api");


        private RootObject jsString = new RootObject();

        [Given(@"alteryx running at""(.*)""")]
        public void GivenAlteryxRunningAt(string SUT_url)
        {
            alteryxurl = Environment.GetEnvironmentVariable(SUT_url);
        }

        [Given(@"I am logged in using ""(.*)"" and ""(.*)""")]
        public void GivenIAmLoggedInUsingAnd(string user, string password)
        {
            _sessionid = Obj.Authenticate(user, password).sessionId;
        }

        [When(
            @"I run the application """"(.*)"""" and geocode a single address """"(.*)"""" , ""(.*)"", ""(.*)"", ""(.*)"" and default values"
            )]
        public void WhenIRunTheApplicationAndGeocodeASingleAddressAndDefaultValues(string app, string address,
            string city, string state, int zip)
        {
            //url + "/apps/gallery/?search=" + appName + "&limit=20&offset=0"
            //Search for App & Get AppId & userId 
            string response = Obj.SearchAppsGallery(app);
            var appresponse =
                new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(
                    response);
            int count = appresponse["recordCount"];
            if (count == 1)
            {
                _appid = appresponse["records"][0]["id"];
                _userid = appresponse["records"][0]["owner"]["id"];
                _appName = appresponse["records"][0]["primaryApplication"]["fileName"];
            }
            else
            {
                for (int i = 0; i <= count - 1; i++)
                {

                    _appActualName = appresponse["records"][i]["primaryApplication"]["metaInfo"]["name"];
                    if (_appActualName == app)
                    {
                        _appid = appresponse["records"][i]["id"];
                        _userid = appresponse["records"][i]["owner"]["id"];
                        _appName = appresponse["records"][i]["primaryApplication"]["fileName"];
                        break;
                    }
                }

            }

            jsString.appPackage.id = _appid;
            jsString.userId = _userid;
            jsString.appName = _appName;

            //url +"/apps/" + appPackageId + "/interface/
            //Get the app interface - not required
            string appinterface = Obj.GetAppInterface(_appid);
            dynamic interfaceresp = JsonConvert.DeserializeObject(appinterface);


            //Construct the payload to be posted.
            List<JsonPayload.Question> questionAnsls = new List<JsonPayload.Question>();
            questionAnsls.Add(new JsonPayload.Question("Geocode Multiple Addresses", "false"));
            questionAnsls.Add(new JsonPayload.Question("Geocode a single address?", "true"));
            questionAnsls.Add(new JsonPayload.Question("SingleAddress", "\"" + address + "\""));
            questionAnsls.Add(new JsonPayload.Question("Single Address City", "\"" + city + "\""));
            questionAnsls.Add(new JsonPayload.Question("Singe State", "\"" + state + "\""));
            questionAnsls.Add(new JsonPayload.Question("Single Zip", "\"" + zip.ToString() + "\""));
            questionAnsls.Add(new JsonPayload.Question("GeocodeAdvFieldsBox", "false"));
            questionAnsls.Add(new JsonPayload.Question("CASSAdvFieldsBox", "false"));
            questionAnsls.Add(new JsonPayload.Question("InputFile", "{\"fileId\":\"\",\"fieldMap\":[]}"));
            // jsString.questions.AddRange(questionAnsls);

            //var inp = new List<JsonPayload.datac>();
            //inp.Add(new JsonPayload.datac() { key = "GEO_GeoLevel", value = "true" });
            //string inpf = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(inp);

            var gc = new List<JsonPayload.datac>();
            gc.Add(new JsonPayload.datac() {key = "GEO_GeoLevel", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_MatchStatus", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_Score", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "SpatialObj", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_Latitude", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_Longitude", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_FirstLine", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_LastLine", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_Interactive", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_FirstLineChanged", value = "true"});
            gc.Add(new JsonPayload.datac() {key = "GEO_LastLineChanged", value = "true"});
            string gcode = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(gc);

            var gca = new List<JsonPayload.datac>();
            gca.Add(new JsonPayload.datac() {key = "GEO_City", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_Province", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_PostalCode", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_HouseNumber", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_StreetPreDir", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_StreetPreType", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_StreetName", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_StreetPostType", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_StreetPostDir", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_UnitDesignator", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_UnitNumber", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_StateAbbrev", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_CensusId", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_ClusterLevelId", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_IntersectingHouseNumber", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_IntersectingStreetPreDir", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_IntersectingStreetPreType", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_IntersectingStreetName", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_IntersectingStreetPostType", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_IntersectingStreetPostDir", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFPreDirChanged", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFStreetNameChanged", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFPostDirChanged", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFUnitDesAssumed", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFAddNumberOutOfRange", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFAddLetterMovedToUnitNum", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFEvenOddAddDiffer", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFSuppliedCity", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFSuppliedState", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFSuppliedPostCode", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFCityChanged", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFStateChanged", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFPostCodeChanged", value = "false"});
            gca.Add(new JsonPayload.datac() {key = "GEO_MFUsedCityAlias", value = "false"});
            string gcodead = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(gca);

            var bcass = new List<JsonPayload.datac>();
            bcass.Add(new JsonPayload.datac() {key = "CASS_Address", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_AddressPlusSuite", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_CarrierRoute", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_City", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_Results", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_DeliveryPointCheckDigit", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_DeliveryPointCode", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_ErrorCode", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_LastLine", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_Plus4", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_State", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_StatusCode", value = "true"});
            bcass.Add(new JsonPayload.datac() {key = "CASS_ZIP", value = "true"});
            string bcasfield = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(bcass);

            var cassa = new List<JsonPayload.datac>();
            cassa.Add(new JsonPayload.datac() {key = "CASS_AddressRange", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_AddressTypeCode", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_AddressTypeString", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_CityAbbreviation", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_CMRA", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_CongressionalDistrict", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_CountyFIPS", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_CountyName", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_ErrorString", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_EWSFlag", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_LACS", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_MSA", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_PMSA", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_PostDirection", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_PreDirection", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_PrivateMailbox", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_PrivateMailboxName", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_PrivateMailboxNumber", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_StreetName", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_Suffix", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_Suite", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_SuiteName", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_SuiteRange", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_SuiteStatus", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_TimeZone", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_TimeZoneCode", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_Urbanization", value = "false"});
            cassa.Add(new JsonPayload.datac() {key = "CASS_ZIPType", value = "false"});
            string casadv = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(cassa);

            var z9 = new List<JsonPayload.datac>();
            z9.Add(new JsonPayload.datac() {key = "Z9_Census_BLOCK", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_Census_ID", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_Census_TRACT", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_County_FIPS", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_Latitude", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_Longitude", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_Match_Code", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_Record_Type", value = "true"});
            z9.Add(new JsonPayload.datac() {key = "Z9_State_FIPS", value = "true"});
            string z9field = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(z9);


            for (int i = 0; i < 6; i++)
            {

                if (i == 0)
                {
                    JsonPayload.Question questionAns = new JsonPayload.Question();
                    questionAns.name = "GeocodeFieldsBasic";
                    questionAns.answer = gcode;
                    jsString.questions.Add(questionAns);
                }
                else if (i == 1)
                {
                    JsonPayload.Question questionAns = new JsonPayload.Question();
                    questionAns.name = "GeocodeFieldsAdvanced";
                    questionAns.answer = gcodead;
                    jsString.questions.Add(questionAns);
                }
                else if (i == 2)
                {
                    JsonPayload.Question questionAns = new JsonPayload.Question();
                    questionAns.name = "BasicCASSField";
                    questionAns.answer = bcasfield;
                    jsString.questions.Add(questionAns);
                }
                else if (i == 3)
                {
                    JsonPayload.Question questionAns = new JsonPayload.Question();
                    questionAns.name = "CASSAdvFields";
                    questionAns.answer = casadv;
                    jsString.questions.Add(questionAns);
                }
                else if (i == 4)
                {
                    JsonPayload.Question questionAns = new JsonPayload.Question();
                    questionAns.name = "Z9Fields";
                    questionAns.answer = z9field;
                    jsString.questions.Add(questionAns);
                }
                else
                {
                    jsString.questions.AddRange(questionAnsls);
                }
            }
            jsString.jobName = "Job Name";

            // Make Call to run app

            var postData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsString);
            string postdata = postData.ToString();
            string resjobqueue = Obj.QueueJob(postdata);

            var jobqueue =
                new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(
                    resjobqueue);
            jobid = jobqueue["id"];

            //Get the job status

            string status = "";
            while (status != "Completed")
            {
                string jobstatusresp = Obj.GetJobStatus(jobid);
                statusresp =
                    new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(
                        jobstatusresp);
                status = statusresp["status"];
                messagecount = statusresp["messages"].Count;
            }
        }

        [Then(@"I see the output ""(.*)""")]
        public void ThenISeeTheOutput(string result)
        {
            //check the output message to see if the output file is generated.
            for (int i = 1; i < messagecount - 1; i++)
            {
                int toolid = statusresp["messages"][i]["toolId"];
                if (toolid == 469)
                {
                    string text = statusresp["messages"][i]["text"];
                    StringAssert.Contains(result,text);
                }
            }
        }
    }
}
