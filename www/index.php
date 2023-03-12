<?php
    function Get_Files_List($dir)
    {   
        $path_array = array();
        
        $dh = new DirectoryIterator($dir);
        
        foreach ($dh as $item)
        {
            if (!$item->isDot())
            {
                if ($item->isDir()) 
                    Get_Files_List("$dir/$item");
                else 
                {
                    $fileName = $item->getFilename();
                    $extension = $item->getExtension();
    
                    // Ignore php, htaccess files in directory listing
                    if ($extension != "php" && $extension != "htaccess")
                    {
                        $path = $dir ."/".$fileName."\r\n";
                        
                        // remove "./" at the begining of the path
                        $path = str_replace("./", "", $path);
    
                        // add path to array
                        array_push($path_array, $path);
                    }
                }
            }
        }
        print "data=".bin2hex(implode($path_array));
    }
    
    function Get_file_md5hash($file_path)
    {
        return md5_file($file_path);
    }  
    
    if(isset($_GET['patches']))
    {
        Get_Files_List("patches");
    }
    else if(isset($_GET['patches_hd']))
    {
        Get_Files_List("patches_hd");
    }
    else if(!empty($_GET['md5']))
    {
        $file_path = $_GET['md5'];
        echo Get_file_md5hash($file_path);
    }
?>