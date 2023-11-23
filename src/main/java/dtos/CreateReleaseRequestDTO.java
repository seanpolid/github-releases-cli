package dtos;

public class CreateReleaseRequestDTO {

	private String tagName;
	private String targetCommitish;
	private String name;
	private String body;
	private boolean draft;
	private boolean prerelease;
	private boolean generateReleaseNotes;
	
	public CreateReleaseRequestDTO(String name, String target) {
		this.tagName = name;
		this.targetCommitish = target;
		this.name = name;
		this.body = null;
		this.draft = false;
		this.prerelease = false;
		this.generateReleaseNotes = false;
	}
	
	public String getTagName() {
		return tagName;
	}
	
	public void setTagName(String tagName) {
		this.tagName = tagName;
	}
	
	public String getTargetCommitish() {
		return targetCommitish;
	}
	
	public void setTargetCommitish(String targetCommitish) {
		this.targetCommitish = targetCommitish;
	}
	
	public String getName() {
		return name;
	}
	
	public void setName(String name) {
		this.name = name;
	}
	
	public String getBody() {
		return body;
	}
	
	public void setBody(String body) {
		this.body = body;
	}
	
	public boolean getDraft() {
		return draft;
	}
	
	public void setDraft(boolean draft) {
		this.draft = draft;
	}
	
	public boolean isPrerelease() {
		return prerelease;
	}
	
	public void setPrerelease(boolean prerelease) {
		this.prerelease = prerelease;
	}
	
	public boolean isGenerateReleaseNotes() {
		return generateReleaseNotes;
	}
	
	public void setGenerateReleaseNotes(boolean generateReleaseNotes) {
		this.generateReleaseNotes = generateReleaseNotes;
	}
	
}
