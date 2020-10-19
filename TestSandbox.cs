using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

public class TestSandbox {

    private static String apiUrl = "https://sandbox-api.gocustomate.com/v1";
    private static String apiKey = "01331fd1-d4d4-4ca4-a457-7635d7ce4a3e";
    private static String apiSecret = "c1e1aaf3ca4b44a7cc634939e74c260893e9d225961ce45098";

    public static void Main() {
        GetAPIStatus();
        //GetAllProfiles();
        //String newId = CreateProfile("bqj18@tester.com", "+447917654403"); // email and phone need to be unique
        //Console.WriteLine("\nnewId: " + newId + "\n");
        //GetProfile(newId);
        //UpdateProfile(newId, "bqj19@tester.com", "+447917654404");
        //DeleteProfile(newId);
    }


    // Encrypts a string using the secret
    private static string CreateToken(string message, string secret)
    {
        secret = secret ?? "";
        var encoding = new System.Text.ASCIIEncoding();
        byte[] keyByte = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            String base64String = Convert.ToBase64String(hashmessage);
            return base64String;
        }
    }


    // GET the status of the Customate sandbox (https://sandbox-api.gocustomate.com/v1/status)
    private static void GetAPIStatus()
    {
        String datetime = DateTime.UtcNow.ToString("s") + "Z";
        Guid guid = Guid.NewGuid();

        String message = "GET\n/v1/status\n\npaymentservice-contenthash:\npaymentservice-date:" + datetime + "\npaymentservice-nonce:" + guid;
        String token = CreateToken(message, apiSecret);
        System.Console.WriteLine("****************************************\n\nRequest\n-------\n" + message);
        System.Console.WriteLine("\nAccess Token:: " + token);

        string url = apiUrl + "/status";
        string output = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers["PaymentService-Date"] = datetime;
        request.Headers["PaymentService-Nonce"] = guid.ToString();
        request.Headers["Authorization"] = "Signature " + apiKey + ":" + token;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            output = reader.ReadToEnd();
        }
        Console.WriteLine("\nResponse\n--------\n" + output + "\n");
    }


    // Get all profiles (GET https://sandbox-api.gocustomate.com/v1/profiles)
    private static void GetAllProfiles() {
        String datetime = DateTime.UtcNow.ToString("s") + "Z";
        Guid guid = Guid.NewGuid();

        String message = "GET\n/v1/profiles\n\npaymentservice-contenthash:\npaymentservice-date:" + datetime + "\npaymentservice-nonce:" + guid;
        String token = CreateToken(message, apiSecret);
        System.Console.WriteLine("****************************************\n\nRequest\n-------\n" + message);
        System.Console.WriteLine("\nAccess Token:: " + token);

        string url = apiUrl + "/profiles";
        string output = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers["PaymentService-Date"] = datetime;
        request.Headers["PaymentService-Nonce"] = guid.ToString();
        request.Headers["Authorization"] = "Signature " + apiKey + ":" + token;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            output = reader.ReadToEnd();
            Console.WriteLine("\nResponse\n--------\n" + output);
        }
    }


    // Creates a profile - the email and phone number need to be unique (POST https://sandbox-api.gocustomate.com/v1/profiles)
    private static String CreateProfile(String email, String phoneNumber) {
        String body =
            "{" +
                "\"type\" : \"personal\", " +
                "\"first_name\" : \"Bobby\", " +
                "\"middle_name\" : \"Quentino\", " +
                "\"last_name\" : \"Jonesy\", " +
                "\"email\" : \"" + email + "\", " +
                "\"phone_number\" : \"" + phoneNumber + "\", " +
                "\"birth_date\" : \"1980-01-02\", " +
                "\"title\" : \"mrs\", " +
                "\"gender\" : \"female\", " +
                "\"address\" : {" +
                    "\"line_1\" : \"118 Boroughbridge Road\", " +
                    "\"line_2\" : \"Near Bourneville\", " +
                    "\"line_3\" : \"Birmingham\", " +
                    "\"city\" : \"Birmingham\", " +
                    "\"locality\" : \"Birmingham\", " +
                    "\"postcode\" : \"B5 7DT\", " +
                    "\"country\" : \"GB\"" +
                "}, " +
                "\"metadata\" : {" +
                    "\"sample_internal_id\" : \"505f7b6e-d0e9-46da-a2f9-e2a737a29d19\"" +
                "}" +
            "}";

        byte[] bytes = Encoding.Default.GetBytes(body);
        String encodedBody = Encoding.UTF8.GetString(bytes);

        String datetime = DateTime.UtcNow.ToString("s") + "Z";
        Guid guid = Guid.NewGuid();

        String message = "POST\n/v1/profiles\napplication/json\npaymentservice-contenthash:\npaymentservice-date:" + datetime + "\npaymentservice-nonce:" + guid;
        String token = CreateToken(message, apiSecret);
        System.Console.WriteLine("****************************************\n\nRequest\n-------\n" + message);
        System.Console.WriteLine("\nAccess Token:: " + token);

        String url = apiUrl + "/profiles";
        String output = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.Headers["PaymentService-Date"] = datetime;
        request.Headers["PaymentService-Nonce"] = guid.ToString();
        request.Headers["Authorization"] = "Signature " + apiKey + ":" + token;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(encodedBody);
        }

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            output = reader.ReadToEnd();
        }
        Console.WriteLine("\nResponse\n--------\n" + output);

        // The output returns: {"id":"<36 character id>", ...
        String newId = output.Substring(7, 36);
        return newId;
    }


    // Get the profile (GET https://sandbox-api.gocustomate.com/v1/profiles/<profile_id>)
    private static void GetProfile(String id) {
        String datetime = DateTime.UtcNow.ToString("s") + "Z";
        Guid guid = Guid.NewGuid();

        String message = "GET\n/v1/profiles/" + id + "\n\npaymentservice-contenthash:\npaymentservice-date:" + datetime + "\npaymentservice-nonce:" + guid;
        String token = CreateToken(message, apiSecret);
        System.Console.WriteLine("****************************************\n\nRequest\n-------\n" + message);
        System.Console.WriteLine("\nAccess Token:: " + token);

        string url = apiUrl + "/profiles/" + id;
        string output = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers["PaymentService-Date"] = datetime;
        request.Headers["PaymentService-Nonce"] = guid.ToString();
        request.Headers["Authorization"] = "Signature " + apiKey + ":" + token;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            output = reader.ReadToEnd();
        }
        Console.WriteLine("\nResponse\n--------\n" + output);
    }


    // Update the profile (PUT https://sandbox-api.gocustomate.com/v1/profiles/<profile_id>)
    private static void UpdateProfile(String id, String email, String phoneNumber) {
        String body =
            "{" +
                "\"id\" : \"" + id + "\", " +
                "\"creation_datetime\" : \"2020-10-19T11:02:29.258312\", " +
                "\"type\" : \"personal\", " +
                "\"first_name\" : \"Bobby\", " +
                "\"middle_name\" : \"Quentino\", " +
                "\"last_name\" : \"Jonesy\", " +
                "\"email\" : \"" + email + "\", " +
                "\"phone_number\" : \"" + phoneNumber + "\", " +
                "\"birth_date\" : \"1980-01-02\", " +
                "\"title\" : \"mrs\", " +
                "\"gender\" : \"female\", " +
                "\"address\" : {" +
                    "\"line_1\" : \"119 Cricket Road\", " +
                    "\"line_2\" : \"Near Edgbaston\", " +
                    "\"line_3\" : \"Edgbaston\", " +
                    "\"city\" : \"Birmingham\", " +
                    "\"locality\" : \"Birmingham\", " +
                    "\"postcode\" : \"B5 7DU\", " +
                    "\"country\" : \"GB\"" +
                "}, " +
                "\"metadata\" : {" +
                    "\"sample_internal_id\" : \"505f7b6e-d0e9-46da-a2f9-e2a737a29d19\"" +
                "}" +
            "}";

        byte[] bytes = Encoding.Default.GetBytes(body);
        String encodedBody = Encoding.UTF8.GetString(bytes);

        String datetime = DateTime.UtcNow.ToString("s") + "Z";
        Guid guid = Guid.NewGuid();

        String message = "PUT\n/v1/profiles/" + id + "\napplication/json\npaymentservice-contenthash:\npaymentservice-date:" + datetime + "\npaymentservice-nonce:" + guid;
        String token = CreateToken(message, apiSecret);
        System.Console.WriteLine("****************************************\n\nRequest\n-------\n" + message);
        System.Console.WriteLine("\nAccess Token:: " + token);

        String url = apiUrl + "/profiles/" + id;
        String output = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "PUT";
        request.ContentType = "application/json";
        request.Headers["PaymentService-Date"] = datetime;
        request.Headers["PaymentService-Nonce"] = guid.ToString();
        request.Headers["Authorization"] = "Signature " + apiKey + ":" + token;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(encodedBody);
        }

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            output = reader.ReadToEnd();
        }
        Console.WriteLine("\nResponse\n--------\n" + output);
    }


    // Delete the profile (DELETE https://sandbox-api.gocustomate.com/v1/profiles/<profile_id>)
    private static void DeleteProfile(String id) {
        String datetime = DateTime.UtcNow.ToString("s") + "Z";
        Guid guid = Guid.NewGuid();

        String message = "DELETE\n/v1/profiles/" + id + "\n\npaymentservice-contenthash:\npaymentservice-date:" + datetime + "\npaymentservice-nonce:" + guid;
        String token = CreateToken(message, apiSecret);
        System.Console.WriteLine("****************************************\n\nRequest\n-------\n" + message);
        System.Console.WriteLine("\nAccess Token:: " + token);

        string url = apiUrl + "/profiles/" + id;
        string output = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "DELETE";
        request.Headers["PaymentService-Date"] = datetime;
        request.Headers["PaymentService-Nonce"] = guid.ToString();
        request.Headers["Authorization"] = "Signature " + apiKey + ":" + token;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            output = reader.ReadToEnd();
        }
        Console.WriteLine("\nResponse\n--------\n" + output);
    }


}
