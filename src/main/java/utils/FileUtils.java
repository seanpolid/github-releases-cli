package utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

public class FileUtils {
	
	private static Pattern versionPattern = Pattern.compile("[0-9]+.[0-9]+.[0-9+]+");
	
	/**
	 * Zips the file or folder found at the provided asset path. 
	 * @param assetPath
	 * @param zipName
	 * @return the path to the zipped asset.
	 * @throws IOException
	 */
	public static String zipAsset(String assetPath, String zipName) throws IOException {
		File pathFile = new File(assetPath);
		String parentPath = pathFile.getParent();
		String zipPath = parentPath + "/" + zipName + ".zip";
		
		FileOutputStream outputStream = new FileOutputStream(new File(zipPath));
		ZipOutputStream zipOutputStream = new ZipOutputStream(outputStream);
		
		zip(pathFile, zipOutputStream, "");
	
		zipOutputStream.close();
		outputStream.close();
		
		return zipPath;
	}
	
	private static void zip(File pathFile, ZipOutputStream zipOutputStream, String basePath) throws IOException {
        if (pathFile.isDirectory()) {
        	File[] files = pathFile.listFiles();
        	
        	String newBaseDir = basePath + pathFile.getName() + "/";
            for (File file : files) {
                zip(file, zipOutputStream, newBaseDir);
            }
        } else {
        	String entryName = basePath + pathFile.getName();
        	zipOutputStream.putNextEntry(new ZipEntry(entryName));

            FileInputStream fileInputStream = new FileInputStream(pathFile);
            byte[] buffer = new byte[1024];
            int len;
            while ((len = fileInputStream.read(buffer)) > 0) {
            	zipOutputStream.write(buffer, 0, len);
            }

            zipOutputStream.closeEntry();
            fileInputStream.close();
        }
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
		if (matcher.find()) {
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
