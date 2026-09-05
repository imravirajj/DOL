import apiClient from "./apiClient";
import type {
  SalesFunnelDto,
  StockAgingDto,
  RevenueAnalyticsDto,
  BrandDto,
  VehicleModelDto,
  VehicleVariantDto,
  VehicleStockDto,
  SalesLeadDto,
  CreateLeadRequest,
  QuotationDto,
  TestDriveBookingDto,
  VehicleBookingDto,
  VehicleOrderDto,
  LoanApplicationDto,
  DeliveryInspectionDto,
  RtoTaxSlabDto,
  PaymentTransactionDto,
  InitiatePaymentRequest,
  CustomerDocumentDto,
  UploadDocumentRequest,
  WarrantyPackageDto,
  VehicleWarrantySubscriptionDto,
  VehicleAccessoryDto,
  CreateAccessoryRequest,
  InsurancePolicyDto,
  InsurancePlanDto,
  VehicleTradeInDto,
  ValuateTradeInRequest,
  ServiceAppointmentDto,
  BookServiceAppointmentRequest,
  EvChargingStationDto,
  HomeChargerInstallationDto,
  CustomerNotificationDto,
  DealershipReviewDto,
  CompanyDto,
  BranchDto,
  CountryDto,
  StateRegionDto,
  CityDto,
} from "../types/dealershipDtos";

// 1. Executive Analytics
export const analyticsApi = {
  getDashboardFunnel: async (companyId?: string, branchId?: string): Promise<SalesFunnelDto> => {
    const params = new URLSearchParams();
    if (companyId) params.append("companyId", companyId);
    if (branchId) params.append("branchId", branchId);
    const qs = params.toString() ? `?${params.toString()}` : "";
    const res = await apiClient.get<SalesFunnelDto>(`/analytics/sales-funnel${qs}`, {
      suppressGlobalError: true,
    });
    return res.data;
  },
  getStockAging: async (companyId?: string, branchId?: string): Promise<StockAgingDto> => {
    const params = new URLSearchParams();
    if (companyId) params.append("companyId", companyId);
    if (branchId) params.append("branchId", branchId);
    const qs = params.toString() ? `?${params.toString()}` : "";
    const res = await apiClient.get<StockAgingDto>(`/analytics/stock-aging${qs}`, {
      suppressGlobalError: true,
    });
    return res.data;
  },
  getRevenueAnalytics: async (companyId?: string, branchId?: string): Promise<RevenueAnalyticsDto> => {
    const params = new URLSearchParams();
    if (companyId) params.append("companyId", companyId);
    if (branchId) params.append("branchId", branchId);
    const qs = params.toString() ? `?${params.toString()}` : "";
    const res = await apiClient.get<RevenueAnalyticsDto>(`/analytics/revenue${qs}`, {
      suppressGlobalError: true,
    });
    return res.data;
  },
};

// 2. Vehicle Catalog
export const catalogApi = {
  getBrands: async (): Promise<BrandDto[]> => {
    const res = await apiClient.get<BrandDto[]>("/catalog/brands");
    return res.data;
  },
  getModels: async (brandId?: string): Promise<VehicleModelDto[]> => {
    const res = await apiClient.get<VehicleModelDto[]>("/catalog/models", {
      params: { brandId },
    });
    return res.data;
  },
  getVariants: async (modelId?: string): Promise<VehicleVariantDto[]> => {
    const res = await apiClient.get<VehicleVariantDto[]>("/catalog/variants", {
      params: { modelId },
    });
    return res.data;
  },
};

// 3. Yard Inventory
export const inventoryApi = {
  getVehicles: async (branchId?: string, status?: string): Promise<VehicleStockDto[]> => {
    const res = await apiClient.get<VehicleStockDto[]>("/inventory/vehicles", {
      params: { branchId, status },
    });
    return res.data;
  },
  updateStatus: async (id: string, status: string): Promise<void> => {
    await apiClient.put(`/inventory/vehicles/${id}/status`, { status });
  },
  recordYardMovement: async (id: string, newLocation: string): Promise<void> => {
    await apiClient.post(`/inventory/vehicles/${id}/move`, { yardLocation: newLocation });
  },
};

// 4. CRM Leads Pipeline
export const crmApi = {
  getLeads: async (priority?: string): Promise<SalesLeadDto[]> => {
    const res = await apiClient.get<SalesLeadDto[]>("/crm/leads", {
      params: { priority },
    });
    return res.data;
  },
  createLead: async (req: CreateLeadRequest): Promise<SalesLeadDto> => {
    const res = await apiClient.post<SalesLeadDto>("/crm/leads", req);
    return res.data;
  },
  updateStage: async (id: string, stage: string, lostReason?: string): Promise<void> => {
    await apiClient.put(`/crm/leads/${id}/stage`, { stage, lostReason });
  },
  scheduleFollowUp: async (id: string, nextDate: string, notes?: string): Promise<void> => {
    await apiClient.post(`/crm/leads/${id}/follow-up`, { nextFollowUpDate: nextDate, notes });
  },
};

// 5. Sales Flow (Quotes, Test Drives, Bookings, Orders)
export const salesFlowApi = {
  getQuotations: async (): Promise<QuotationDto[]> => {
    const res = await apiClient.get<QuotationDto[]>("/quotations");
    return res.data;
  },
  createQuotation: async (data: any): Promise<QuotationDto> => {
    const res = await apiClient.post<QuotationDto>("/quotations", data);
    return res.data;
  },
  getTestDrives: async (): Promise<TestDriveBookingDto[]> => {
    const res = await apiClient.get<TestDriveBookingDto[]>("/test-drives");
    return res.data;
  },
  bookTestDrive: async (data: any): Promise<TestDriveBookingDto> => {
    const res = await apiClient.post<TestDriveBookingDto>("/test-drives", data);
    return res.data;
  },
  updateTestDriveStatus: async (id: string, status: string, feedbackNotes?: string): Promise<void> => {
    await apiClient.put(`/test-drives/${id}/status`, { status, feedbackNotes });
  },
  getBookings: async (): Promise<VehicleBookingDto[]> => {
    const res = await apiClient.get<VehicleBookingDto[]>("/bookings");
    return res.data;
  },
  createBooking: async (data: any): Promise<VehicleBookingDto> => {
    const res = await apiClient.post<VehicleBookingDto>("/bookings", data);
    return res.data;
  },
  getOrders: async (): Promise<VehicleOrderDto[]> => {
    const res = await apiClient.get<VehicleOrderDto[]>("/orders");
    return res.data;
  },
  createOrder: async (data: any): Promise<VehicleOrderDto> => {
    const res = await apiClient.post<VehicleOrderDto>("/orders", data);
    return res.data;
  },
  updateOrderStatus: async (id: string, status: string): Promise<void> => {
    await apiClient.put(`/orders/${id}/status`, { status });
  },
};

// 6. Finance, Payments, Insurance & Trade-In
export const financeApi = {
  getPayments: async (): Promise<PaymentTransactionDto[]> => {
    const res = await apiClient.get<PaymentTransactionDto[]>("/payments");
    return res.data;
  },
  initiatePayment: async (req: InitiatePaymentRequest): Promise<PaymentTransactionDto> => {
    const res = await apiClient.post<PaymentTransactionDto>("/payments/initiate", req);
    return res.data;
  },
  refundPayment: async (id: string, reason?: string): Promise<void> => {
    await apiClient.post(`/payments/${id}/refund`, { reason });
  },
  getLoans: async (): Promise<LoanApplicationDto[]> => {
    const res = await apiClient.get<LoanApplicationDto[]>("/loans");
    return res.data;
  },
  applyLoan: async (data: any): Promise<LoanApplicationDto> => {
    const res = await apiClient.post<LoanApplicationDto>("/loans", data);
    return res.data;
  },
  getInsurancePolicies: async (): Promise<InsurancePolicyDto[]> => {
    const res = await apiClient.get<InsurancePolicyDto[]>("/insurance/policies");
    return res.data;
  },
  getInsurancePlans: async (): Promise<InsurancePlanDto[]> => {
    const res = await apiClient.get<InsurancePlanDto[]>("/insurance/plans");
    return res.data;
  },
  getTradeIns: async (): Promise<VehicleTradeInDto[]> => {
    const res = await apiClient.get<VehicleTradeInDto[]>("/exchange/trade-ins");
    return res.data;
  },
  valuateTradeIn: async (req: ValuateTradeInRequest): Promise<VehicleTradeInDto> => {
    const res = await apiClient.post<VehicleTradeInDto>("/exchange/valuate", req);
    return res.data;
  },
};

// 7. KYC Customer Documents
export const documentsApi = {
  getDocuments: async (userId?: string): Promise<CustomerDocumentDto[]> => {
    const res = await apiClient.get<CustomerDocumentDto[]>("/documents", {
      params: { userId },
    });
    return res.data;
  },
  uploadDocument: async (req: UploadDocumentRequest): Promise<CustomerDocumentDto> => {
    const res = await apiClient.post<CustomerDocumentDto>("/documents/upload", req);
    return res.data;
  },
  verifyDocument: async (id: string, approve: boolean, rejectionReason?: string): Promise<void> => {
    await apiClient.put(`/documents/${id}/verify`, { approve, rejectionReason });
  },
};

// 8. Aftersales, Warranty, Accessories, RTO, Delivery, Service & EV
export const aftersalesApi = {
  getAccessories: async (category?: string): Promise<VehicleAccessoryDto[]> => {
    const res = await apiClient.get<VehicleAccessoryDto[]>("/accessories", {
      params: { category },
    });
    return res.data;
  },
  createAccessory: async (req: CreateAccessoryRequest): Promise<VehicleAccessoryDto> => {
    const res = await apiClient.post<VehicleAccessoryDto>("/accessories", req);
    return res.data;
  },
  getWarrantyPackages: async (): Promise<WarrantyPackageDto[]> => {
    const res = await apiClient.get<WarrantyPackageDto[]>("/warranty/packages");
    return res.data;
  },
  getWarrantySubscriptions: async (): Promise<VehicleWarrantySubscriptionDto[]> => {
    const res = await apiClient.get<VehicleWarrantySubscriptionDto[]>("/warranty/subscriptions");
    return res.data;
  },
  getRtoTaxSlabs: async (): Promise<RtoTaxSlabDto[]> => {
    const res = await apiClient.get<RtoTaxSlabDto[]>("/rto/tax-slabs");
    return res.data;
  },
  getDeliveries: async (): Promise<DeliveryInspectionDto[]> => {
    const res = await apiClient.get<DeliveryInspectionDto[]>("/deliveries");
    return res.data;
  },
  completeDeliveryPdi: async (id: string, isPdiPassed: boolean): Promise<void> => {
    await apiClient.put(`/deliveries/${id}/pdi`, { isPdiPassed });
  },
  getServiceAppointments: async (): Promise<ServiceAppointmentDto[]> => {
    const res = await apiClient.get<ServiceAppointmentDto[]>("/service/appointments");
    return res.data;
  },
  bookServiceAppointment: async (req: BookServiceAppointmentRequest): Promise<ServiceAppointmentDto> => {
    const res = await apiClient.post<ServiceAppointmentDto>("/service/appointments", req);
    return res.data;
  },
  getChargingStations: async (): Promise<EvChargingStationDto[]> => {
    const res = await apiClient.get<EvChargingStationDto[]>("/ev/charging-stations");
    return res.data;
  },
  getHomeChargers: async (): Promise<HomeChargerInstallationDto[]> => {
    const res = await apiClient.get<HomeChargerInstallationDto[]>("/ev/home-chargers");
    return res.data;
  },
};

// 9. Organization & Masters
export const adminMasterApi = {
  getCompanies: async (): Promise<CompanyDto[]> => {
    const res = await apiClient.get<CompanyDto[]>("/companies");
    return res.data;
  },
  getBranches: async (companyId?: string): Promise<BranchDto[]> => {
    const res = await apiClient.get<BranchDto[]>("/branches", {
      params: { companyId },
    });
    return res.data;
  },
  getCountries: async (): Promise<CountryDto[]> => {
    const res = await apiClient.get<CountryDto[]>("/locations/countries");
    return res.data;
  },
  getStates: async (countryId?: string): Promise<StateRegionDto[]> => {
    const res = await apiClient.get<StateRegionDto[]>("/locations/states", {
      params: { countryId },
    });
    return res.data;
  },
  getCities: async (stateId?: string): Promise<CityDto[]> => {
    const res = await apiClient.get<CityDto[]>("/locations/cities", {
      params: { stateId },
    });
    return res.data;
  },
};

// 10. Feedback & Reviews
export const feedbackApi = {
  getNotifications: async (): Promise<CustomerNotificationDto[]> => {
    const res = await apiClient.get<CustomerNotificationDto[]>("/notifications");
    return res.data;
  },
  markRead: async (id: string): Promise<void> => {
    await apiClient.put(`/notifications/${id}/read`);
  },
  getReviews: async (): Promise<DealershipReviewDto[]> => {
    const res = await apiClient.get<DealershipReviewDto[]>("/reviews");
    return res.data;
  },
};
