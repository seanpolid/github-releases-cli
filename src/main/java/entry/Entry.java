package entry;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.CommandLineParser;
import org.apache.commons.cli.DefaultParser;
import org.apache.commons.cli.HelpFormatter;
import org.apache.commons.cli.Options;

import orchestrators.Orchestrator;
import utils.FileUtil;

public class Entry  {
	
    public static void main(String[] args) {
    	Options options = createOptions();
    	CommandLine cmd = parseArgs(options, args);
    	if (cmd == null) {return;}

    	if (!cmd.hasOption("p") || !FileUtil.isValidPath(cmd.getOptionValue("p"))) {
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
    	
    	try {
    		Orchestrator.run(cmd.getOptionValue("p"), cmd.getOptionValue("r"), cmd.getOptionValue("n"), repoOwner, githubToken);
    	} catch (Exception ex) {
    		System.out.println("An unexpected error occurred while trying to publish a release: " + ex.getMessage());
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

	private static CommandLine parseArgs(Options options, String[] args) {
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
    
}
