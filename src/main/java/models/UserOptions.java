package models;

public class UserOptions {

	private String assetPath;
	private String repositoryName;
	private String zipName;
	private String version;
	
	public UserOptions(String assetPath, String repositoryName, String zipName, String version) {
		super();
		this.assetPath = assetPath;
		this.repositoryName = repositoryName;
		this.zipName = zipName;
		this.version = version;
		formatZipName();
	}

	public String getAssetPath() {
		return assetPath;
	}

	public String getRepositoryName() {
		return repositoryName;
	}

	public String getZipName() {
		return zipName;
	}

	public String getVersion() {
		return version;
	}
	
	public void formatZipName() {
		if (zipName != null && zipName.contains(".zip")) {
			zipName = zipName.substring(0, zipName.indexOf(".zip"));
		}
	}
	
}
