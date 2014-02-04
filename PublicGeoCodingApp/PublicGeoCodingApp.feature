Feature: PublicGeoCodingApp
	Assign geographic coordinates to your address records accurately and efficiently.

 Background:
  Given alteryx running at" http://gallery.alteryx.com/"
  And I am logged in using "deepak.manoharan@accionlabs.com" and "P@ssw0rd"

Scenario Outline: Run the app and assign geo codes
When I run the application "<app>" and geocode a single address "<Address>" , "<city>", "<State>", "<Zip>" and default values
Then I see the output "<result>"
Examples: 
| app                            | Address         | city    | State | Zip   | result                       |
| "Alteryx Public Geocoding App" | "3825 Iris Ave" | Boulder | CO    | 80026 | "GeocodeResults.yxdb" |



