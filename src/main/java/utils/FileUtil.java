package utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

public class FileUtil {
	
	private static Pattern versionPattern = Pattern.compile("[0-9]+.[0-9]+.[0-9+]+");
	
	/**
	 * Zips the file or folder found at the provided asset path. 
	 * @param assetPath
	 * @param zipName
	 * @return the path to the zipped asset.
	 * @throws IOException
	 */
	public static String zipAsset(String assetPath, String zipName) throws IOException {
		File file = new File(assetPath);
		String parentPath = file.getParent();
		String zipPath = parentPath + "/" + zipName + ".zip";
		
		FileInputStream inputStream = new FileInputStream(assetPath);
		FileOutputStream outputStream = new FileOutputStream(new File(zipPath));
		ZipOutputStream zipOutputStream = new ZipOutputStream(outputStream);
		
		ZipEntry entry = new ZipEntry(file.getName());
		zipOutputStream.putNextEntry(entry);
		
		int length;
		byte[] bytes = new byte[1024];
		while ((length = inputStream.read(bytes)) != -1) {
			zipOutputStream.write(bytes, 0, length);
		}
		
		inputStream.close();
		zipOutputStream.close();
		outputStream.close();
		
		return zipPath;
	}
	
	/**
	 * Determines if the provided path exists on the current machine.
	 * @param path
	 * @return true if the path is valid, false otherwise
	 */
	public static boolean isValidPath(String path) {
		File file = new File(path);
		if (file.isAbsolute() && file.exists()) {
			return true;
		}
		
		file = new File(System.getProperty("user.dir") + "/" + path);
		if (file.exists()) {
			return true;
		}
		
		return false;
	}

	/**
	 * Attempts to find the version by recursively checking all folders and files
	 * found at or within the provided path. Looks for a pattern that matches #.#.#.
	 * @param path
	 * @return the version, or null if not found.
	 */
	public static String findVersion(String path) {
		Matcher matcher = versionPattern.matcher(path);
		if (matcher.matches()) {
			return matcher.group();
		}
		
		File pathFile = new File(path);
		if (pathFile.isDirectory()) {
			File[] files = pathFile.listFiles();
			
			for (File file : files) {
				String version = findVersion(file.getPath());
				if (version != null) {
					return version;
				}
			}
		}
		
		return null;
	}
	
}
