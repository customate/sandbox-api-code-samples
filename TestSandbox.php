<?php

getStatus();
//getAllProfiles();
//$newId = createProfile("bqj24@tester.com", "+447917654422");
//echo "\nnewId: $newId\n";
//getProfile($newId);
//updateProfile($newId, "bqj25@tester.com", "+447917654423");
//deleteProfile($newId);


// Get the status of the Customate sandbox (GET https://sandbox-api.gocustomate.com/v1/status)
function getStatus() {
    $date = (new Datetime()) -> format('Y-m-d\TH:i:s\Z');
    $uuid = getGUID();

    $string_to_sign = utf8_encode("GET\n/v1/status\n\npaymentservice-contenthash:\npaymentservice-date:$date\npaymentservice-nonce:$uuid");
    echo "****************************************\n\nRequest\n-------\n$string_to_sign\n";

    $access_token = createToken($string_to_sign, getApiSecret());
    echo "\nAccess Token: $access_token\n";
    $signature = getApiKey() . ":" . $access_token;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, getApiUrl()."/status");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "PaymentService-Date:$date",
        "PaymentService-Nonce:$uuid",
        "Authorization: Signature $signature"
    ]);

    $response = curl_exec($ch);
    echo "\nResponse\n--------\n";
    var_dump($response);
    curl_close($ch);
}


// Get profiles (https://sandbox-api.gocustomate.com/v1/profiles?page[number]=1&page[size]=99)
function getAllProfiles() {
    $date = (new Datetime()) -> format('Y-m-d\TH:i:s\Z');
    $uuid = getGUID();
    $string_to_sign = utf8_encode("GET\n/v1/profiles\n\npaymentservice-contenthash:\npaymentservice-date:$date\npaymentservice-nonce:$uuid");

    echo "\n****************************************\n\nRequest\n-------\n$string_to_sign\n";
    $access_token = createToken($string_to_sign, getApiSecret());
    echo "\nAccess Token: $access_token\n";
    $signature = getApiKey() . ":" . $access_token;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, getApiUrl()."/profiles");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "PaymentService-Date:$date",
        "PaymentService-Nonce:$uuid",
        "Authorization: Signature $signature"
    ]);

    $response = curl_exec($ch);
    echo "\nResponse\n--------\n";
    var_dump($response);
    curl_close($ch);
}


// Create a profile - email and phone need to be unique (POST https://sandbox-api.gocustomate.com/v1/profiles)
function createProfile($email, $phone_number) {
    $date = (new Datetime()) -> format('Y-m-d\TH:i:s\Z');
    $uuid = getGUID();

    $body = [
      "type" => "personal",
      "first_name" => "Bob",
      "middle_name" => "Quentin",
      "last_name" => "Jones",
      "email" => $email,
      "phone_number" => $phone_number,
      "birth_date" => "1980-01-01",
      "title" => "mr",
      "gender" => "male",
      "address" => [
        "line_1" => "117 Boroughbridge Road",
        "line_2" => "Near Edgbaston",
        "line_3" => "Edgbaston",
        "city" => "Birmingham",
        "locality" => "Birmingham",
        "postcode" => "B5 7DS",
        "country" => "GB",
      ],
      "metadata" => [
        "sample_internal_id" => "505f7b6e-d0e9-46da-a2f9-e2a737a29d19"
      ]
    ];

    $requestBody = json_encode($body);
    $contentHashSource = utf8_encode($requestBody);
    $string_to_sign = utf8_encode("POST\n/v1/profiles\napplication/json\npaymentservice-contenthash:\npaymentservice-date:$date\npaymentservice-nonce:$uuid");

    echo "****************************************\n\nRequest\n-------\n$string_to_sign\n";
    $access_token = createToken($string_to_sign, getApiSecret());
    $signature = getApiKey() . ":" . $access_token;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, getApiUrl()."/profiles");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_POST, TRUE);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "Content-Type: application/json",
        "PaymentService-Date:$date",
        "PaymentService-Nonce:$uuid",
        "Authorization: Signature $signature"
    ]);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $contentHashSource);

    $response = curl_exec($ch); // The response returns: {"id":"<36 character id>", ...
    $newId = substr($response, 7, 36);
    echo "\nResponse\n--------\n";
    var_dump($response);
    curl_close($ch);
    return $newId;
}


// Get the profile - set $string_to_sign and CURLOPT_URL to the one just created (https://sandbox-api.gocustomate.com/v1/profiles/<profile_id>)
function getProfile($id) {
    $date = (new Datetime()) -> format('Y-m-d\TH:i:s\Z');
    $uuid = getGUID();
    $string_to_sign = utf8_encode("GET\n/v1/profiles/$id\n\npaymentservice-contenthash:\npaymentservice-date:$date\npaymentservice-nonce:$uuid");

    echo "\n****************************************\n\nRequest\n-------\n$string_to_sign\n";
    $access_token = createToken($string_to_sign, getApiSecret());
    echo "\nAccess Token: $access_token\n";
    $signature = getApiKey() . ":" . $access_token;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, getApiUrl()."/profiles/$id");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "PaymentService-Date:$date",
        "PaymentService-Nonce:$uuid",
        "Authorization: Signature $signature"
    ]);

    $response = curl_exec($ch);
    echo "\nResponse\n--------\n";
    var_dump($response);
    curl_close($ch);
}


// Update a profile (PUT https://sandbox-api.gocustomate.com/v1/profiles/<profile_id>)
function updateProfile($id, $email, $phone_number) {
    $date = (new Datetime()) -> format('Y-m-d\TH:i:s\Z');
    $uuid = getGUID();

    $body = [
      "type" => "personal",
      "first_name" => "Bobby",
      "middle_name" => "Quentino",
      "last_name" => "Jones",
      "email" => $email,
      "phone_number" => $phone_number,
      "birth_date" => "1980-01-02",
      "title" => "mrs",
      "gender" => "female",
      "address" => [
        "line_1" => "118 Boroughbridge Road",
        "line_2" => "Near Bournville",
        "line_3" => "Bournville",
        "city" => "Birmingham",
        "locality" => "Birmingham",
        "postcode" => "B5 7DT",
        "country" => "GB",
      ],
      "metadata" => [
        "sample_internal_id" => "505f7b6e-d0e9-46da-a2f9-e2a737a29d19"
      ]
    ];

    $requestBody = json_encode($body);
    $contentHashSource = utf8_encode($requestBody);
    $string_to_sign = utf8_encode("PUT\n/v1/profiles/$id\napplication/json\npaymentservice-contenthash:\npaymentservice-date:$date\npaymentservice-nonce:$uuid");

    echo "\n****************************************\n\nRequest\n-------\n$string_to_sign\n";
    $access_token = createToken($string_to_sign, getApiSecret());
    $signature = getApiKey() . ":" . $access_token;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, getApiUrl()."/profiles/$id");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'PUT');
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "Content-Type: application/json",
        "PaymentService-Date:$date",
        "PaymentService-Nonce:$uuid",
        "Authorization: Signature $signature"
    ]);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $contentHashSource);

    $response = curl_exec($ch);
    echo "\nResponse\n--------\n";
    var_dump($response);
    curl_close($ch);
}


// Delete the profile (DELETE https://sandbox-api.gocustomate.com/v1/profiles/<profile_id>)
function deleteProfile($id) {
    $date = (new Datetime()) -> format('Y-m-d\TH:i:s\Z');
    $uuid = getGUID();
    $string_to_sign = utf8_encode("DELETE\n/v1/profiles/$id\n\npaymentservice-contenthash:\npaymentservice-date:$date\npaymentservice-nonce:$uuid");

    echo "\n****************************************\n\nRequest\n-------\n$string_to_sign\n";
    $access_token = createToken($string_to_sign, getApiSecret());
    echo "\nAccess Token: $access_token\n";
    $signature = getApiKey() . ":" . $access_token;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, getApiUrl()."/profiles/$id");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "DELETE");
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "PaymentService-Date:$date",
        "PaymentService-Nonce:$uuid",
        "Authorization: Signature $signature"
    ]);

    $response = curl_exec($ch);
    echo "\nResponse\n--------\n";
    var_dump($response);
    curl_close($ch);
}


// Generate a Guid (from: https://stackoverflow.com/questions/21671179/how-to-generate-a-new-guid)
function getGUID() {
    if (function_exists('com_create_guid')){
        return com_create_guid();
    } else {
        mt_srand((double) microtime() * 10000); // Optional for php 4.2.0 and up.
        $charid = strtoupper(md5(uniqid(rand(), true)));
        $hyphen = chr(45); // "-"
        $uuid = ""
            .substr($charid,  0,  8).$hyphen
            .substr($charid,  8,  4).$hyphen
            .substr($charid, 12,  4).$hyphen
            .substr($charid, 16,  4).$hyphen
            .substr($charid, 20, 12);
        return $uuid;
    }
}


function getApiUrl() {
    return 'https://sandbox-api.gocustomate.com/v1';
}


function getApiKey() {
    return '01331fd1-d4d4-4ca4-a457-7635d7ce4a3e';
}


function getApiSecret() {
    return 'c1e1aaf3ca4b44a7cc634939e74c260893e9d225961ce45098';
}


// Encrypts a string using the secret
function createToken($message, $secret) {
    $hmac_hash = hash_hmac("sha256", $message, $secret);
    $access_token = utf8_decode(base64_encode(pack('H*', $hmac_hash)));
    return $access_token;
}
?>
