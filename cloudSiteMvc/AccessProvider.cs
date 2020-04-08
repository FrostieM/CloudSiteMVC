using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace cloudSiteMvc
{
    public enum AccessType { ApiKey, User, ServiceAccount };

    public class AccessProvider
    {
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string _applicationName = SpreadsheetConfigReader.ApplicationName;
        private readonly string _spreadsheetId = SpreadsheetConfigReader.SpreadsheetId;
        private readonly string _serviceAccount = SpreadsheetConfigReader.ServiceAccount;
        private readonly string _apiKey = SpreadsheetConfigReader.ApiKey;
        private readonly SheetsService _service;

        public AccessProvider(AccessType accessType)
        {
            _service = GetSheetsService(accessType);
        }

        public bool WriteData(IList<IList<object>> data, string sheetName)
        {            
            if (!HasSheet(sheetName))
                    CreateNewSheet(sheetName);

            return ClearSheet(sheetName) && AppendEntries(data, sheetName);
        }

        public bool AppendEntries(IEnumerable<IEnumerable<object>> data, string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var valueRange = new ValueRange();

            valueRange.Values = data.Cast<IList<object>>().ToList();

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            AppendValuesResponse response;
            try { response = appendRequest.Execute(); }
            catch (Exception e)
            {
                Console.WriteLine("AppendEntries method exception:\n" + e.Message);
                response = null;
            }

            return response != null;
        }

        public bool ClearSheet(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var requestBody = new ClearValuesRequest();

            var deleteRequest = _service.Spreadsheets.Values.Clear(requestBody, _spreadsheetId, range);

            ClearValuesResponse response;
            try { response = deleteRequest.Execute(); }
            catch (Exception e)
            {
                Console.WriteLine("ClearSheet method exception:\n" + e.Message);
                response = null;
            }

            return response != null;
        }
        
        public bool CreateNewSheet(string sheetName)
        {
            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                }
            };
            var requests = new List<Request> { addSheetRequest };

            var batchUpdate = new BatchUpdateSpreadsheetRequest();
            batchUpdate.Requests = requests;

            BatchUpdateSpreadsheetResponse response;
            try { response = _service.Spreadsheets.BatchUpdate(batchUpdate, _spreadsheetId).Execute(); }
            catch (Exception e)
            {
                Console.WriteLine("CreateNewSheet method exception:\n" + e.Message);
                response = null;
            }

            return response != null;
        }

        public IEnumerable<IEnumerable<string>> ReadEntries(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var response = _service.Spreadsheets.Values.Get(_spreadsheetId, range).Execute();
            var values = response.Values.Select(list => list.Select(listItem => listItem.ToString()));
            return values;
        }

        public IEnumerable<string> GetSheetNames()
        {
            var spreadsheet = _service.Spreadsheets.Get(_spreadsheetId).Execute();
            return spreadsheet.Sheets.Select(sheet => sheet.Properties.Title).ToList();
        }

        public bool HasSheet(string sheetName)
        {
            return GetSheetId(sheetName) != -1;
        }

        public int GetSheetId(string sheetName)
        {
            var spreadsheet = _service.Spreadsheets.Get(_spreadsheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);

            if (sheet == null)
                return -1;

            var sheetId = (int) sheet.Properties.SheetId;
            return sheetId;
        }
        
        private UserCredential GetUserCredential()
        {
            using (var stream = new FileStream("configs/client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetCreds.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    "User",
                    CancellationToken.None,
                    new FileDataStore(credentialPath, true)).Result;
            }
        }

        private ServiceAccountCredential GetServiceAccountCredential()
        {
            using (Stream stream = new FileStream(HttpContext.Current.Server.MapPath("~") + "configs/service_account_secret.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var credential = (ServiceAccountCredential)
                    GoogleCredential.FromStream(stream).UnderlyingCredential;

                var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                {
                    HttpClientFactory = GetHttpClientFactory(),
                    User = _serviceAccount,
                    Key = credential.Key,
                    Scopes = _scopes
                };
                return new ServiceAccountCredential(initializer);
            }
        }

        private SheetsService GetSheetsService(ICredential credential)
        {
            return new SheetsService(
                new BaseClientService.Initializer()
                {
                    HttpClientFactory = GetHttpClientFactory(),
                    HttpClientInitializer = credential,
                    ApplicationName = _applicationName
                });
        }

        private SheetsService GetSheetsService()
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientFactory = GetHttpClientFactory(),
                ApplicationName = _applicationName,
                ApiKey = _apiKey
            });
        }

        private SheetsService GetSheetsService(AccessType type)
        {
            if (type == AccessType.ServiceAccount || type == AccessType.User)
                return type == AccessType.ServiceAccount ?
                    GetSheetsService(GetServiceAccountCredential()) :
                    GetSheetsService(GetUserCredential());            
            else            
                return GetSheetsService();            
        }

        private HttpClientFactory GetHttpClientFactory()
        {
            return new HttpClientFactory();
        }
    }
}