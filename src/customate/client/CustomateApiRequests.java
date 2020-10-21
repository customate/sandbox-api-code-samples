package customate.client;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpRequest.Builder;
import java.net.http.HttpResponse;
import java.net.http.HttpResponse.BodyHandlers;
import java.security.InvalidKeyException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.Base64;
import java.util.Formatter;
import java.util.Map;
import java.util.Optional;
import java.util.TreeMap;
import java.util.UUID;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

public class CustomateApiRequests {

	private static final HttpClient httpClient = HttpClient.newBuilder().build();

	private static final String key = System.getenv().get("CUSTOMATE_API_KEY");
	private static final String secret = System.getenv().get("CUSTOMATE_API_SECRET");

	public static Optional<HttpResponse<String>> doGet(String url) throws URISyntaxException {

		URI uri = new URI(url);

		Builder requestBuilder = HttpRequest.newBuilder().uri(uri).GET();

		return sendRequest(uri, "GET", requestBuilder, "");
	}

	public static Optional<HttpResponse<String>> doPost(String url, String bodyContent) throws URISyntaxException {

		String contentHash = contentHash(bodyContent);

		URI uri = new URI(url);

		Builder requestBuilder = HttpRequest.newBuilder().uri(uri)
				.POST(HttpRequest.BodyPublishers.ofString(bodyContent));

		return sendRequest(uri, "POST", requestBuilder, contentHash);
	}

	public static Optional<HttpResponse<String>> doDelete(String url) throws URISyntaxException {

		URI uri = new URI(url);

		Builder requestBuilder = HttpRequest.newBuilder().uri(uri).DELETE();

		return sendRequest(uri, "DELETE", requestBuilder, "");
	}

	private static Optional<HttpResponse<String>> sendRequest(URI uri, String method, Builder requestBuilder,
			String contentHash) {

		String UUIDString = getUUIDString();

		String dateString = getDate("+01:00");

		String headersString = headersToString(contentHash, dateString, UUIDString);

		String contentType = method.equals("POST") || method.equals("PUT") ? "application/json" : "";

		String accessTokenString = accessTokenToString(method, uri.getPath(), contentType, headersString);

		Optional<String> accessToken = createToken(secret.getBytes(), accessTokenString.getBytes());

		String authorizationString = getAuthString(accessToken.get());

		HttpResponse<String> response = null;

		if (accessToken.isPresent()) {

			try {

				requestBuilder.header("Authorization", authorizationString);
				requestBuilder.header("paymentservice-date", dateString);
				requestBuilder.header("paymentservice-nonce", UUIDString);

				if (!contentHash.isEmpty()) {
					requestBuilder.header("paymentservice-contenthash", contentHash);
					requestBuilder.header("Content-Type", contentType);
				}

				HttpRequest request = requestBuilder.build();

				response = httpClient.send(request, BodyHandlers.ofString());
				
				System.out.println("\nResponse\n--------\n" + response);				

			} catch (IOException | InterruptedException e) {
				e.printStackTrace();
			}

		}
		return Optional.ofNullable(response);
	}

	private static String contentHash(String bodyContent) {
		String contentHash = "";
		try {
			MessageDigest crypt = MessageDigest.getInstance("SHA-1");
			crypt.reset();
			crypt.update(bodyContent.getBytes("UTF-8"));
			byte[] hash = crypt.digest();

			Formatter formatter = new Formatter();
			for (byte b : hash) {
				formatter.format("%02x", b);
			}
			contentHash = formatter.toString();

			formatter.close();
		} catch (NoSuchAlgorithmException | UnsupportedEncodingException e) {
			e.printStackTrace();
		}

		return contentHash;
	}

	private static Optional<String> createToken(byte[] secret, byte[] message) {
		String token = null;
		try {
			Mac mac = Mac.getInstance("HmacSHA256");
			SecretKeySpec secretKeySpec = new SecretKeySpec(secret, "HmacSHA256");
			mac.init(secretKeySpec);
			byte[] bytes = mac.doFinal(message);
			token = Base64.getEncoder().encodeToString(bytes);
		} catch (NoSuchAlgorithmException | InvalidKeyException e) {
			e.printStackTrace();
		}
		return Optional.ofNullable(token);
	}

	private static String headersToString(String contentHash, String paymentServiceDateString,
			String paymentServiceNonceString) {

		Map<String, String> headersMap = new TreeMap<>();
		headersMap.put("paymentservice-contenthash", contentHash);
		headersMap.put("paymentservice-date", paymentServiceDateString);
		headersMap.put("paymentservice-nonce", paymentServiceNonceString);

		StringBuilder headersString = new StringBuilder();
		headersMap.forEach((K, V) -> headersString.append(K).append(":").append(V).append("\n"));

		return headersString.toString().trim();
	}

	private static String accessTokenToString(String method, String path, String contentType, String headers) {
		return new StringBuilder(method).append('\n').append(path).append('\n').append(contentType).append('\n')
				.append(headers).toString();
	}

	private static String getAuthString(String hashedString) {
		return new StringBuilder("Signature ").append(key).append(":").append(hashedString).toString();
	}

	private static String getUUIDString() {
		return UUID.randomUUID().toString();
	}

	private static String getDate(String offset) {
		return LocalDateTime.now(ZoneId.of("Europe/London")).withNano(0).toString().concat(offset);
	}
}
