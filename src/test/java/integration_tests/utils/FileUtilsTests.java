package integration_tests.utils;

import static org.junit.jupiter.api.Assertions.assertArrayEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.junit.jupiter.api.Assertions.assertAll;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Stream;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.Test;

import utils.FileUtils;

public class FileUtilsTests {

	private String rootPath = System.getProperty("user.dir") + "/src/test/java/integration_tests/test_files";
	
	@AfterEach
	public void cleanUp() {
		List<File> filesToRemove = new ArrayList<>();
		findFilesToRemove(rootPath, filesToRemove);
		
		for (File fileToRemove : filesToRemove) {
			if (fileToRemove.isDirectory()) {
				deleteNestedFiles(fileToRemove);
			}
			
			fileToRemove.delete();
		}
	}

	private void findFilesToRemove(String rootPath, List<File> filesToRemove) {
		File[] files = new File(rootPath).listFiles();
		
		for (File file : files) {
			if (file.isDirectory()) {
				if (file.getPath().contains("assets")) {
					filesToRemove.add(file);
				}
				
				findFilesToRemove(file.getAbsolutePath(), filesToRemove);
			} else if (file.getPath().contains(".zip") || file.getPath().contains("assets")) {
				filesToRemove.add(file);
			}
		}
	}
	
	private void deleteNestedFiles(File fileToRemove) {
		if (fileToRemove.isDirectory()) {
			for (File file : fileToRemove.listFiles()) {
				deleteNestedFiles(file);
			}
		} else {
			fileToRemove.delete();
		}
	}

	@Test
	public void test_zipAsset_folder_success() throws IOException {
		// Arrange
		String assetPath = rootPath + "/success_path";
		
		// Act
		String zipPath = FileUtils.zipAsset(assetPath, "assets");
		
		// Assert
		String unzipPath = unzip(zipPath, assetPath);
		assertAll(() -> {
			assertFolderPathsEqual(unzipPath, assetPath);
		});
	}
	
	private void assertFolderPathsEqual(String unzipStartPath, String assetPath) {	
		List<String[]> unzipPathNestedFilesNames = new ArrayList<>();
		List<String[]> assetPathNestedFilesNames = new ArrayList<>();
		
		String unzipPath = new File(unzipStartPath).listFiles()[0].getPath();
		
		File unzipPathFile = new File(unzipPath);
		File assetPathFile = new File(assetPath);
		
		unzipPathNestedFilesNames.add(new String[] {unzipPathFile.getName()});
		assetPathNestedFilesNames.add(new String[] {assetPathFile.getName()});
		
		addNestedFiles(unzipPathFile, unzipPathNestedFilesNames);
		addNestedFiles(assetPathFile, assetPathNestedFilesNames);
		
		assertArrayEquals(assetPathNestedFilesNames.toArray(), unzipPathNestedFilesNames.toArray());
	}

	private void addNestedFiles(File pathFile, List<String[]> nestedFiles) {
		if (pathFile.isFile()) {return;}
		
		File[] files = pathFile.listFiles();
		Arrays.sort(files, (file1, file2) -> file1.compareTo(file2));
		
		String[] fileNames = Stream.of(files)
								   .map(file -> file.getName())
								   .toArray(String[]::new);
		nestedFiles.add(fileNames);
		
		if (pathFile.isDirectory()) {
			for (File file : files) {
				addNestedFiles(file, nestedFiles);
			}
			
		}
	}

	private String unzip(String zipPath, String assetPath) throws IOException {
		File zipFile = new File(zipPath);
		FileInputStream fileInputStream = new FileInputStream(zipPath);
		ZipInputStream zipInputStream = new ZipInputStream(fileInputStream);
		
		String parentPath = zipFile.getPath().substring(0, zipFile.getPath().length() - 4);
		File parentPathFile = new File(parentPath);
		if (!parentPathFile.exists()) {
			parentPathFile.mkdir();
		}
		
		ZipEntry entry;
		while((entry = zipInputStream.getNextEntry()) != null) {
			String fileName = new File(entry.getName()).getName();
			if (!entry.getName().contains(".")) {
				File file = new File(parentPath + "/" + fileName);
				file.mkdir();
				continue;
			}
			
			FileOutputStream fileOutputStream = new FileOutputStream(parentPath + "/" + entry.getName());
			
			int bytesRead;
			byte[] buffer = new byte[1024];
			while((bytesRead = zipInputStream.read(buffer, 0, buffer.length)) != -1) {
				fileOutputStream.write(buffer);
			}
			
			fileOutputStream.close();
		}
		
		zipInputStream.close();
		fileInputStream.close();
		
		return parentPath;
	}

	@Test
	public void test_zipAsset_file_success() throws IOException {
		// Arrange
		String assetPath = rootPath + "/success_path/v1.0.2.txt";

		// Act
		String zipPath = FileUtils.zipAsset(assetPath, "assets");
		
		// Assert
		String unzipPath = unzip(zipPath, assetPath);
		assertAll(() -> {
			assertFolderPathsEqual(unzipPath, assetPath);
		});
	}
	
	
	@Test
	public void test_isValidPath_true() {
		// Arrange
		String path = rootPath + "/success_path";
		
		// Act
		boolean isValidPath = FileUtils.isValidPath(path);
		
		// Assert
		assertTrue(isValidPath);
	}
	
	@Test
	public void test_isValidPath_false() {
		// Arrange
		String path = rootPath + "/nonexistant_folder";
		
		// Act
		boolean isValidPath = FileUtils.isValidPath(path);
		
		// Assert
		assertFalse(isValidPath);
	}
	
	@Test
	public void test_findVersion_success() {
		// Act
		String version = FileUtils.findVersion(rootPath + "/success_path");
		
		// Assert
		assertEquals("1.0.2", version);
	}
	
	@Test
	public void test_findVersion_null() {
		// Act
		String version = FileUtils.findVersion(rootPath + "/failure_path");
		
		// Assert
		assertNull(version);
	}
	
}
