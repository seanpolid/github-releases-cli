package dtos;

public class CreateReleaseResponseDTO {

	private String url;
	private String uploadUrl;
	private String assetUrl;

	public String getUrl() {
		return url;
	}

	public void setUrl(String url) {
		this.url = url;
	}

	public String getUploadUrl() {
		return uploadUrl;
	}

	public void setUploadUrl(String uploadUrl) {
		this.uploadUrl = uploadUrl;
	}

	public String getAssetUrl() {
		return assetUrl;
	}

	public void setAssetUrl(String assetUrl) {
		this.assetUrl = assetUrl;
	}

	@Override
	public String toString() {
		return "CreateReleaseResponseDTO [url=" + url + ", uploadUrl=" + uploadUrl + ", assetUrl=" + assetUrl + "]";
	}
	
}
