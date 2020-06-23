import lib2to3
from lib2to3.pgen2.parse import ParseError
from lib2to3.refactor import RefactoringTool, get_fixers_from_package
import zipfile
import os.path
import sys

def transform(source):
    # CHANGE THIS!!!!
    zip_path = "C:\\Users\\SylvesterKnudsen\\AppData\\Local\\python-3.7.3-embed-amd64"
    #zip_path = os.path.dirname(sys.executable)
    zip_file = get_zip_file(zip_path)
    zip_folder = zipfile.ZipFile(os.path.join(zip_path, zip_file))
    fixers = get_all_fixers_from_zipfolder(zip_folder)

    refactoring_tool = RefactoringTool(fixers)

    return refactor_script(source, refactoring_tool)

def refactor_script(source_script, refactoring_tool):
    # lib2to3 likes a newline at the end
    source_script += '\n'
    try:
        tree = refactoring_tool.refactor_string(source_script, 'script')
    except ParseError as e:
        if e.msg != 'bad input' or e.value != '=':
            raise
        tree = refactoring_tool.refactor_string(source_script, self.pathname)
    return str(tree)[:-1] # remove added newline 

def get_zip_file(path):
    zip_file = ""
    for f in os.listdir(path):
        if f.startswith("python") and f.endswith(".zip"):
            zip_file = f
            break
    return zip_file

def get_all_fixers_from_zipfolder(folder):
    fixers = []
    files = folder.filelist
    for f in files:
        if "fix_" not in f.filename:
            continue
        fixers.append(f.filename.replace('/', '.')[:-4])
    return fixers

#original_script = open("C:\\Users\\SylvesterKnudsen\\Desktop\\old",'r').read()
output = transform(code)