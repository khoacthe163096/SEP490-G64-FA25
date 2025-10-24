using FE.vn.fpt.edu.viewmodels;
using FE.vn.fpt.edu.adapters;

namespace FE.vn.fpt.edu.services
{
    public class ComponentService
    {
        private readonly ApiAdapter _apiAdapter;

        public ComponentService(ApiAdapter apiAdapter)
        {
            _apiAdapter = apiAdapter;
        }

        public async Task<ComponentListViewModel> GetComponentsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, long? typeComponentId = null, string? statusFilter = null)
        {
            try
            {
                var queryString = $"Component?page={page}&pageSize={pageSize}";
                
                if (!string.IsNullOrEmpty(searchTerm))
                    queryString += $"&search={Uri.EscapeDataString(searchTerm)}";
                
                if (typeComponentId.HasValue)
                    queryString += $"&typeComponentId={typeComponentId.Value}";

                var response = await _apiAdapter.GetAsync<dynamic>(queryString);
                
                if (response.success)
                {
                    var components = new List<ComponentViewModel>();
                    foreach (var item in response.data)
                    {
                        components.Add(new ComponentViewModel
                        {
                            Id = item.id,
                            Name = item.name,
                            Code = item.code,
                            UnitPrice = item.unitPrice,
                            QuantityStock = item.quantityStock,
                            ImageUrl = item.imageUrl,
                            TypeComponentId = item.typeComponentId,
                            TypeComponentName = item.typeComponentName,
                            BranchId = item.branchId,
                            BranchName = item.branchName
                        });
                    }

                    return new ComponentListViewModel
                    {
                        Components = components,
                        CurrentPage = response.pagination.page,
                        PageSize = response.pagination.pageSize,
                        TotalCount = response.pagination.totalCount,
                        TotalPages = response.pagination.totalPages,
                        SearchTerm = searchTerm,
                        TypeComponentId = typeComponentId,
                        StatusFilter = statusFilter
                    };
                }
                else
                {
                    throw new Exception(response.message ?? "Lỗi khi tải dữ liệu");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách linh kiện: {ex.Message}");
            }
        }

        public async Task<ComponentViewModel?> GetComponentByIdAsync(long id)
        {
            try
            {
                var response = await _apiAdapter.GetAsync<dynamic>($"Component/{id}");
                
                if (response.success)
                {
                    var item = response.data;
                    return new ComponentViewModel
                    {
                        Id = item.id,
                        Name = item.name,
                        Code = item.code,
                        UnitPrice = item.unitPrice,
                        QuantityStock = item.quantityStock,
                        ImageUrl = item.imageUrl,
                        TypeComponentId = item.typeComponentId,
                        TypeComponentName = item.typeComponentName,
                        BranchId = item.branchId,
                        BranchName = item.branchName
                    };
                }
                else
                {
                    throw new Exception(response.message ?? "Không tìm thấy linh kiện");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin linh kiện: {ex.Message}");
            }
        }

        public async Task<bool> CreateComponentAsync(CreateComponentViewModel model)
        {
            try
            {
                var requestData = new
                {
                    name = model.Name,
                    code = model.Code,
                    unitPrice = model.UnitPrice,
                    quantityStock = model.QuantityStock,
                    typeComponentId = model.TypeComponentId,
                    branchId = model.BranchId,
                    imageUrl = model.ImageUrl
                };

                var response = await _apiAdapter.PostAsync<dynamic>("Component", requestData);
                return response.success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo linh kiện: {ex.Message}");
            }
        }

        public async Task<bool> UpdateComponentAsync(long id, UpdateComponentViewModel model)
        {
            try
            {
                var requestData = new
                {
                    name = model.Name,
                    code = model.Code,
                    unitPrice = model.UnitPrice,
                    quantityStock = model.QuantityStock,
                    typeComponentId = model.TypeComponentId,
                    branchId = model.BranchId,
                    imageUrl = model.ImageUrl
                };

                var response = await _apiAdapter.PutAsync<dynamic>($"Component/{id}", requestData);
                return response.success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật linh kiện: {ex.Message}");
            }
        }

        public async Task<bool> DeleteComponentAsync(long id)
        {
            try
            {
                var success = await _apiAdapter.DeleteAsync($"Component/{id}");
                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa linh kiện: {ex.Message}");
            }
        }

        public async Task<bool> UpdateStockAsync(long id, int quantityStock)
        {
            try
            {
                var requestData = new
                {
                    quantityStock = quantityStock
                };

                var response = await _apiAdapter.PutAsync<dynamic>($"Component/{id}/stock", requestData);
                return response.success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật số lượng: {ex.Message}");
            }
        }

        public async Task<bool> UpdateStatusAsync(long id, string status)
        {
            try
            {
                var requestData = new
                {
                    status = status
                };

                var response = await _apiAdapter.PutAsync<dynamic>($"Component/{id}/status", requestData);
                return response.success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }
    }
}
