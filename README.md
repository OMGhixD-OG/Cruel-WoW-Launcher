#### Cruel-WoW-Launcher | C# | WPF | .NET FRAMEWORK 4.6.1 | Visual Studio 2019

#### Cruel WoW Launcher for OMGhixD#2839

----------------------------------------------------

### How to setup the launcher

1. Pull the source

2. Upload the content of ``Cruel WoW Launcher/wwww`` to your webserver for example in ``https://cruel-wow.com/launcher`` 
but could be any folder name

3. Open and edit the uploaded **Launcher.xml**
    * Change ``remote_path="http://localhost/cruel-launcher"`` to path on your webserver where the content of www folder you uploaded 
for example ``https://cruel-wow.com/launcher`` ?


4. You will need **Visual Studio Community 2019** with **.NET desktop development**

![IMG](https://image.prntscr.com/image/zR5gq3U-RF6NDJzYq3JVGQ.png)

5. Open **Cruel WoW Launcher/Cruel WoW Launcher.sln**

6. Go to ``Project -> Cruel WoW Launcher Properties -> Settings``

7. In the **Value** column add the link to your **Launcher.xml** for example ``https://cruel-wow.com/launcher/Launcher.xml`` but you could upload the file anywhere on web just make sure its public and you can edit it later

----------------------------------------------------

### How to push new client updates

1. Upload new or modified files in the launcher folder specified by the **remote_path** in your **Launcher.xml**
    * For example regular files goes in **https://cruel-wow.com/launcher/patches** 
    * For example HD Graphics files goes in **https://cruel-wow.com/launcher/patches_hd**
    ```
     ! Please note that the "patches" and "patches_hd" folders follows the original paths as in the World of Warcraft folder
     meaning that if you want to add a new file to be downloaded in "World of Warcraft/Data/enUS/realmlist.wtf" then you have to upload it in
     "https://cruel-wow.com/launcher/patches/Data/enUS/realmlist.wtf" or the "patches_hd" folder if you are uploading hd graphics files.
    ```


2. Edit **Launcher.xml**
    * Increase or decrease the **client_version="1"**

3. Save **Launcher.xml** and done.

Now the next time the players starts the launcher they will be requested to update the client.

----------------------------------------------------

### How to change news

1. Open and edit the uploaded **Launcher.xml**

```xml
    <News enable="true">
        <Image>https://github.com/CyberMist2/MyPublicRepo/blob/master/Layer%2010.png?raw=true</Image>
        <Article>https://github.com/CyberMist2/MyPublicRepo/raw/master/Article.rtf</Article>
        <Date>15 August 2020</Date>
        <ReadMore>https://www.cruel-wow.com/en/news/5</ReadMore>
    </News>
```

2. Explanation
      * The **Image tag** holds the url of the news header image
      * The **Article tag** holds the url of the news Article.rtf
      * The **Date tag** holds the news date
      * The **ReadMore tag** holds the link when you press the Read more button on your launcher 
