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
    	
    	String githubOwner = System.getenv("GITHUB_OWNER");
    	if (githubOwner == null || githubOwner.isBlank()) {
    		System.out.println("Sorry, could not identify the GitHub owner. Please ensure the following environment variable has been set: GITHUB_OWNER");
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
    	System.out.println(githubOwner);
    	System.out.println(githubToken);
    }
    
}
