package customate.client.test;

import static org.junit.Assert.*;

import java.net.URISyntaxException;
import java.net.http.HttpResponse;
import java.util.Optional;

import org.junit.Test;

import customate.client.CustomateApiRequests;

public class CustomateApiRequestsTests {

	@Test
	public void testStatus() throws URISyntaxException {
		Optional<HttpResponse<String>> response = CustomateApiRequests
				.doGet("https://sandbox-api.gocustomate.com/v1/status");
		assertTrue(response.isPresent());
		assertEquals(200, response.get().statusCode());
	}

	@Test
	public void testCreateProfile() throws URISyntaxException {
		Optional<HttpResponse<String>> response = CustomateApiRequests
				.doPost("https://sandbox-api.gocustomate.com/v1/profiles", profileJson);

		assertTrue(response.isPresent());

		assertEquals(201, response.get().statusCode());
		
	}
	
	//\"id\":\"41453d1e-74c5-46a2-81c3-d1f09a63297b
	@Test
	public void testGetProfile() throws URISyntaxException {
		Optional<HttpResponse<String>> response = CustomateApiRequests
				.doGet("https://sandbox-api.gocustomate.com/v1/profiles/41453d1e-74c5-46a2-81c3-d1f09a63297b");
		assertTrue(response.isPresent());
		assertEquals(200, response.get().statusCode());
	}
	
	
	@Test
	public void testDeleteProfile() throws URISyntaxException {
		Optional<HttpResponse<String>> response = CustomateApiRequests
				.doDelete("https://sandbox-api.gocustomate.com/v1/profiles/41453d1e-74c5-46a2-81c3-d1f09a63297b");
		assertTrue(response.isPresent());
		assertEquals(204, response.get().statusCode());
	}
	

	String profileJson = "{\r\n" + "    \"type\":  \"personal\",\r\n" + "    \"metadata\":  {\r\n"
			+ "                     \"sample_internal_id\":  \"505f7b6e-d0e9-46da-a2f9-e2a737a29d19\"\r\n"
			+ "                 },\r\n" + "    \"middle_name\":  \"Quentin\",\r\n" + "    \"title\":  \"mr\",\r\n"
			+ "    \"birth_date\":  \"1980-01-01\",\r\n" + "    \"first_name\":  \"Bob\",\r\n"
			+ "    \"address\":  {\r\n" + "                    \"city\":  \"Birmingham\",\r\n"
			+ "                    \"line_3\":  \"Edgbaston\",\r\n"
			+ "                    \"locality\":  \"Birmingham\",\r\n"
			+ "                    \"line_1\":  \"117 Boroughbridge Road\",\r\n"
			+ "                    \"postcode\":  \"B5 7DS\",\r\n"
			+ "                    \"line_2\":  \"Near Edgbaston\",\r\n"
			+ "                    \"country\":  \"GB\"\r\n" + "                },\r\n"
			+ "    \"gender\":  \"male\",\r\n" + "    \"phone_number\":  \"+447917654389\",\r\n"
			+ "    \"last_name\":  \"Jones\",\r\n" + "    \"email\":  \"abcdef@tester.com\"\r\n" + "}";
}
