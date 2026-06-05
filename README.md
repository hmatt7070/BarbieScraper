# Barbie Scraper
### Background
 When my grandmother passed away, I had to take inventory of her Barbie doll collection (roughly 360 dolls). I didn't
 want to manually type the information for each doll, so I created a tool to search for the doll using the MPN and
 [BarbiePedia](https://en.barbiepedia.com/). I then realized I had no idea what anything was worth, so I implemented an
 API for eBay that would search for sold listings and find a rough average and median for the price. All of this
 information was written to a csv file for me to reference. 
 
### Built With
* **Language:** C#
* **Web Scraping:** AngleSharp
* **API Integration:** RapidAPI (eBay Sold Listing)
* **Data Handling:** Automated CSV export and data formatting