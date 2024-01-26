import os
import shutil
import subprocess
import sys
import time


def runMainApp():
    exePath = os.path.abspath('.') + "\\MMClipboard.exe"
    if not (os.path.exists(exePath)):
        return
    subprocess.Popen(exePath)


def moveFile(old_path, new_path):
    if not os.path.exists(oldP):
        return
    filelist = os.listdir(old_path)
    for file in filelist:
        # 源文件
        src = os.path.join(old_path, file)
        # 目标文件
        dst = os.path.join(new_path, file)
        if os.path.isdir(src):

            if not os.path.exists(dst):
                os.makedirs(dst, True)
            cfiles = os.listdir(src)
            for c in cfiles:
                # 源文件
                sourceFileP = os.path.join(old_path, file + '\\' + c)
                # 目标文件
                targetFileP = os.path.join(new_path, file + '\\' + c)
                if os.path.exists(targetFileP):
                    os.remove(targetFileP)
                shutil.move(sourceFileP, targetFileP)
        else:
            if os.path.exists(dst):
                os.remove(dst)
            shutil.move(src, dst)


# 按间距中的绿色按钮以运行脚本。
if __name__ == '__main__':
    if len(sys.argv) > 1:
        if sys.argv[1] == 'com.ht.mmclipboard':
            time.sleep(1)
            oldP = os.path.abspath('.') + '\\Temp\\'
            newP = os.path.abspath('.')
            moveFile(oldP, newP)
            runMainApp()
            shutil.rmtree(oldP)
