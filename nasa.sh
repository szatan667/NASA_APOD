#!/bin/bash

#wait for the network?
sleep 30

#save images in this location
saveDir=$HOME'/Pictures/NASAImages/'
mkdir -p $saveDir

#build proper URL for API
apiURL="https://api.nasa.gov/planetary/apod?api_key=DFihYXvddhhd1KnnPtw3BgSxAXlx9yHz1CSTwbN8&hd=true&date="`date +%Y-%m-%d`

#extract picture URL: 
#1. get json string from API
#2. find "hdurl" tag along with the value
#3. cut out the value with double-quote as delimiter
picURL=`curl -s $apiURL -o - | grep -o "\"hdurl\"\:\".*\.jpg\"\," | cut -d "\"" -f 4`

# get picture filename from picture URL (string after last '/' character?)
picName=${picURL##*/}

#debug
echo $saveDir
echo $apiURL
echo $picURL
echo $picName

# Download picture ($picURL) to save location ($saveDir$picName)
curl -o $saveDir$picName $picURL

# Set the GNOME3 wallpaper from saved picture
DISPLAY=:0 GSETTINGS_BACKEND=dconf gsettings set org.gnome.desktop.background picture-uri '"file://'$saveDir$picName'"'
# Set the GNOME 3 wallpaper picture options
picOpts="scaled"
DISPLAY=:0 GSETTINGS_BACKEND=dconf gsettings set org.gnome.desktop.background picture-options $picOpts

# Exit the script
exit
