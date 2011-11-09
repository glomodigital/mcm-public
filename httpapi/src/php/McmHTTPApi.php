<?php

send("myusername", "mypassword", "myoriginator", "mymsisdn", "mybody", false, null);

function send($username, $password, $originator, $msisdn, $body, $dlr, $ref) {
   $endpoint = "http://mcm.globalmouth.com:8080/api/mcm";
   $request = "?";
   $request .= "username=" . urlencode($username) . "&";
   $request .= "body=" . urlencode($body) . "&";
   $request .= "msisdn=" . urlencode($msisdn) . "&";
   $request .= "dlr=" . urlencode($dlr) . "&";
   if ("dlr" == "true") {
      $request .= "ref=" . $ref . "&";
   }
   if ($originator != null && $originator != "") {
      $request .= "originator=" . $originator . "&";
   }

   $hash = computeHash($username, $password, array($body, $originator, $msisdn));
   $request .= "hash=" . $hash;

   readfile($endpoint . $request);
}

function computeHash($username, $password, $values = array()) {
   $hash = $username;
   for ($i = 0; $i < count($values); $i++) {
      if ($values[$i] != null)
         $hash .= $values[$i];
   }
   $pwHash = md5($username . ":" . $password);
   return md5($hash . $pwHash);
}

?>
