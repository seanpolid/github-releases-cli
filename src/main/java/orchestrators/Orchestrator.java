package orchestrators;

import clients.GitHubClient;
import models.UserOptions;
import utils.FileUtils;

public class Orchestrator {
	
	private static String defaultZipName = "assets";

	public static void run(UserOptions options, String repoOwner, String githubToken) throws Exception {
		String zipName = options.getZipName() == null ? defaultZipName : options.getZipName();
		
		GitHubClient gitHubClient = new GitHubClient(repoOwner, githubToken);
		
		System.out.println("Creating release");
		String uploadUri = gitHubClient.createRelease(options.getRepositoryName(), zipName, options.getVersion());
		
		System.out.println("Zipping asset(s)");
		String zippedAssetPath = FileUtils.zipAsset(options.getAssetPath(), zipName);
		
		System.out.println("Uploading asset(s)");
		gitHubClient.uploadReleaseAsset(uploadUri, zippedAssetPath);
	}

}
