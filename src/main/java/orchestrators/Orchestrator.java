package orchestrators;

import clients.GitHubClient;

public class Orchestrator {
	
	private static String defaultZipName = "assets";

	public static void run(String assetPath, String repositoryName, String zipName, String repoOwner, String githubToken) throws Exception {
		zipName = zipName == null ? defaultZipName : formatZipName(zipName);
		
		GitHubClient gitHubClient = new GitHubClient(repoOwner, githubToken);
		
		// Get latest release version
		// Create release with next version in sequence
		gitHubClient.createRelease(repositoryName, zipName);
		
		System.out.println(assetPath);
    	System.out.println(repositoryName);
    	System.out.println(repoOwner);
    	System.out.println(githubToken);
	}
	
	private static String formatZipName(String zipName) {
		if (zipName.contains(".zip")) {
			return zipName.substring(0, zipName.indexOf(".zip"));
		}
		
		return zipName;
	}
}
