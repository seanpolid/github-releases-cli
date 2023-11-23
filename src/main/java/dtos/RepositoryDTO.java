package dtos;

import java.util.List;

public class RepositoryDTO {

	private String name;
	private List<String> topics;

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public List<String> getTopics() {
		return topics;
	}

	public void setTopics(List<String> topics) {
		this.topics = topics;
	}

	@Override
	public String toString() {
		return "RepositoryDTO [name=" + name + ", topics=" + topics + "]";
	}

}
