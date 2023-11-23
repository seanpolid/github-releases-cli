package utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

public class FileUtil {

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
	
}
