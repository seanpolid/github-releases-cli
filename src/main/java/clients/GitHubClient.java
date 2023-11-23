package clients;

import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.lang.reflect.Type;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpRequest.BodyPublisher;
import java.net.http.HttpRequest.BodyPublishers;
import java.net.http.HttpResponse;
import java.net.http.HttpResponse.BodyHandlers;
import java.security.cert.CertificateExpiredException;
import java.security.cert.CertificateNotYetValidException;
import java.security.cert.X509Certificate;

import javax.net.ssl.SSLSession;

import com.google.gson.FieldNamingPolicy;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;

import dtos.CreateReleaseRequestDTO;
import dtos.CreateReleaseResponseDTO;
import enums.AcceptType;
import enums.ContentType;

public class GitHubClient {
	
	private final String REPO_OWNER;
	private final String GITHUB_TOKEN;
	private HttpClient httpClient = HttpClient.newHttpClient();
	private Gson serializationGson = new GsonBuilder().setFieldNamingPolicy(FieldNamingPolicy.LOWER_CASE_WITH_UNDERSCORES).create();
	
	public GitHubClient(String repoOwner, String githubToken) {
		REPO_OWNER = repoOwner;
		GITHUB_TOKEN = githubToken;
	}
	
	public String createRelease(String repositoryName, String zipName) throws URISyntaxException, CertificateExpiredException, CertificateNotYetValidException, IOException, InterruptedException {
		String uri = "https://api.github.com/repos/" + REPO_OWNER + "/" + repositoryName + "/releases";
		String method = "POST";
		CreateReleaseRequestDTO releaseRequestDTO = new CreateReleaseRequestDTO("v1.0.2", "master");

		BodyPublisher body = BodyPublishers.ofString(serializationGson.toJson(releaseRequestDTO));
		Type type = new TypeToken<CreateReleaseResponseDTO>() {}.getType();
		
		HttpRequest request = createRequest(uri, method, body, AcceptType.GITHUB_JSON, ContentType.JSON);
		CreateReleaseResponseDTO response = send(request, type);
		
		String uploadUrl = response.getUploadUrl();
		return uploadUrl.substring(0, uploadUrl.indexOf('{')) + "?name=" + zipName + "&label=" + zipName;
	}
	
	private HttpRequest createRequest(String uri, String method, BodyPublisher body, AcceptType acceptType, ContentType contentType) throws URISyntaxException {
		return HttpRequest.newBuilder()
				 .uri(new URI(uri))
				 .header("Accept", getAcceptTypeString(acceptType))
				 .header("Authorization", "Bearer " + GITHUB_TOKEN)
				 .header("X-GitHub-Api-Version", "2022-11-28")
				 .header("Content-Type", getContentTypeString(contentType))
				 .method(method, body)
				 .build();
	}

	private String getAcceptTypeString(AcceptType acceptType) {
		switch (acceptType) {
			case GITHUB_JSON:
				return "application/vnd.github+json";
			case OCTET_STREAM:
				return "application/octet-stream";
			default:
				return "application/json";
		}	
	}
	
	private String getContentTypeString(ContentType contentType) {
		switch (contentType) {
			case ZIP:
				return "application/zip";
			case JSON:
				return "application/json";
			default:
				return "";
		}
	}

	private <T> T send(HttpRequest request, Type type) throws IOException, InterruptedException, CertificateExpiredException, CertificateNotYetValidException {
		HttpResponse<String> response = httpClient.send(request, BodyHandlers.ofString());
		SSLSession sslSession = response.sslSession().get();

		X509Certificate[] certificates = (X509Certificate[]) sslSession.getPeerCertificates();
		for (X509Certificate certificate : certificates) {
			certificate.checkValidity();
		}
	
		return serializationGson.fromJson(response.body(), type);
	}
	
	public void uploadReleaseAsset(String uri, String assetPath) throws IOException, URISyntaxException, CertificateExpiredException, CertificateNotYetValidException, InterruptedException {
		String method = "POST";

		BufferedInputStream stream = new BufferedInputStream(new FileInputStream(assetPath));
		BodyPublisher bytes = BodyPublishers.ofByteArray(stream.readAllBytes());
		
		Type type = new TypeToken<CreateReleaseResponseDTO>() {}.getType();
		
		HttpRequest request = createRequest(uri, method, bytes, AcceptType.GITHUB_JSON, ContentType.ZIP);
		send(request, type);
		
		stream.close();
	}
	
	
	
}
