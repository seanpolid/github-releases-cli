package utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

public class FileUtil {

	public static String zipAsset(String assetPath) throws IOException {
		File file = new File(assetPath);
		String parentPath = file.getParent();
		String zipPath = parentPath + "/build.zip";
		
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
	
}
