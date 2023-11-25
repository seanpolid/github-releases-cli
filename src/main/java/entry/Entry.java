package entry;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.CommandLineParser;
import org.apache.commons.cli.DefaultParser;
import org.apache.commons.cli.HelpFormatter;
import org.apache.commons.cli.Options;

import orchestrators.Orchestrator;
import utils.FileUtils;
import models.UserOptions;

public class Entry  {
	
    public static void main(String[] args) {
    	CommandLine cmd = parseArgs(args);
    	if (cmd == null) {return;}
    	
    	String assetPath = getAssetPath(cmd);
    	if (assetPath == null) {return;}
    	
    	String version = getVersion(cmd);
    	if (version == null) {return;}
    	
    	String repoOwner = getRepoOwner();
    	if (repoOwner == null) {return;}
    	
    	String gitHubToken = getGitHubToken();
    	if (gitHubToken == null) {return;}
    	
    	UserOptions userOptions = new UserOptions(assetPath, cmd.getOptionValue("r"), cmd.getOptionValue("n"), version);
    	
    	run(userOptions, repoOwner, gitHubToken);
    }

	private static CommandLine parseArgs(String[] args) {
		Options options = createOptions();
		try {
			CommandLineParser parser = new DefaultParser();
	    	CommandLine cmd = parser.parse(options, args);
	    	return cmd;
		} catch (Exception ex) {
			for (String arg : args) {
				if (arg.contains("-h") || arg.contains("--help")) {
					HelpFormatter formatter = new HelpFormatter();
		    		formatter.printHelp("release", "Publish a release to GitHub", options, null, true);
		    		return null;
				}
			}
			
			System.out.println("release: " + ex.getMessage() + "\nTry 'release --help' for more information.");
			return null;
		}
	}
	
	private static Options createOptions() {
    	Options options = new Options();
    	options.addRequiredOption("p", "path", true, "Path to asset(s)");
    	options.addRequiredOption("r", "repo", true, "Name of repository");
    	options.addOption("n", "name", true, "Name of zip file");
    	options.addOption("h", "help", false, "Help menu");
    	options.addOption("v", "version", true, "Version of release");
    	return options; 
    }
	
	private static String getAssetPath(CommandLine cmd) {
		if (!cmd.hasOption("p") || !FileUtils.isValidPath(cmd.getOptionValue("p"))) {
    		System.out.println("The provided asset path is invalid.");
    		return null;
    	}
		
		return cmd.getOptionValue("p");
	}
	
	private static String getVersion(CommandLine cmd) {
		String version = cmd.getOptionValue("v");
    	if (!cmd.hasOption("v")) {
    		version = FileUtils.findVersion(cmd.getOptionValue("p"));
    		
    		if (version == null) {
    			System.out.println("Could not determine appropriate version for release. Please provide a version or ensure it is present within/at the asset path.");
    		}
    	}
    	
		return version;
	}
	
	private static String getRepoOwner() {
		String repoOwner = System.getenv("REPO_OWNER");
    	if (repoOwner == null || repoOwner.isBlank()) {
    		System.out.println("Sorry, could not identify the owner of the repository. Please ensure the following environment variable has been set: REPO_OWNER");
    		return null;
    	}
    	
		return repoOwner;
	}
	
	private static String getGitHubToken() {
		String gitHubToken = System.getenv("GITHUB_TOKEN");
    	if (gitHubToken == null || gitHubToken.isBlank()) {
    		System.out.println("Sorry, could not identify the GitHub token. Please ensure the following environment variable has been set: GITHUB_TOKEN");
    		return null;
    	}
    	
		return gitHubToken;
	}
    
	private static void run(UserOptions userOptions, String repoOwner, String gitHubToken) {
		try {
    		Orchestrator.run(userOptions, repoOwner, gitHubToken);
    	} catch (Exception ex) {
    		System.out.println("An unexpected error occurred while trying to publish the release: " + ex.getMessage());
    	}
	}
}
