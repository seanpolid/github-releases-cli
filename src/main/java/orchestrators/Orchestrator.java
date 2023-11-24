package orchestrators;

import clients.GitHubClient;
import models.UserOptions;

public class Orchestrator {
	
	private static String defaultZipName = "assets";

	public static void run(UserOptions options, String repoOwner, String githubToken) throws Exception {
		String zipName = options.getZipName() == null ? defaultZipName : options.getZipName();
		
		GitHubClient gitHubClient = new GitHubClient(repoOwner, githubToken);
		gitHubClient.createRelease(options.getRepositoryName(), options.getZipName(), options.getVersion());
		
		System.out.println(options.getAssetPath());
    	System.out.println(options.getRepositoryName());
    	System.out.println(repoOwner);
    	System.out.println(githubToken);
	}

}
