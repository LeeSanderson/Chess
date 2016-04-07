param($installPath, $toolsPath, $package)
$DTE.ItemOperations.Navigate("http://xiopod.net/useful.utilities/?" + $package.Id + "=" + $package.Version)