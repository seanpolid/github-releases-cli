package entry;

import utils.FileUtil;

public class Entry  {
	
    public static void main( String[] args ) {
    	if (args.length != 2) {
    		System.out.println("./release <Asset Path> <Repository Name>");
    		return;
    	}
    	
    	String asssetPath = args[0];
    	if (!FileUtil.isValidPath(asssetPath)) {
    		System.out.println("The provided asset path is invalid.");
    		return;
    	}
    	
    	String repoOwner = System.getenv("REPO_OWNER");
    	if (repoOwner == null || repoOwner.isBlank()) {
    		System.out.println("Sorry, could not identify the owner of the repository. Please ensure the following environment variable has been set: REPO_OWNER");
    		return;
    	}
    	
    	String githubToken = System.getenv("GITHUB_TOKEN");
    	if (githubToken == null || githubToken.isBlank()) {
    		System.out.println("Sorry, could not identify the GitHub token. Please ensure the following environment variable has been set: GITHUB_TOKEN");
    		return;
    	}
    	
    	String repo = args[1];
    	
    	System.out.println(asssetPath);
    	System.out.println(repo);
    	System.out.println(repoOwner);
    	System.out.println(githubToken);
    }
    
}
