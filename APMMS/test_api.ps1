# Test API để kiểm tra dữ liệu trả về
$uri = "https://localhost:7173/api/VehicleCheckin?page=1&pageSize=10"

try {
    $response = Invoke-RestMethod -Uri $uri -Method Get -SkipCertificateCheck
    Write-Host "API Response:"
    $response | ConvertTo-Json -Depth 3
    
    if ($response.data -and $response.data.Count -gt 0) {
        Write-Host "`nFirst item details:"
        $response.data[0] | ConvertTo-Json -Depth 2
    }
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}

