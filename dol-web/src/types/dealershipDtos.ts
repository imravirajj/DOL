// TypeScript definitions for DealerOneLane Enterprise Modules

export type UUID = string;

// Common Enums
export type TradeInStatus = 'Submitted' | 'Valuated' | 'InspectionScheduled' | 'Offered' | 'Accepted' | 'Rejected' | 'Completed';
export type AccessoryCategory = 'Exterior' | 'Interior' | 'Electronics' | 'Safety' | 'CarCare' | 'Lifestyle';
export type InsurancePolicyStatus = 'Draft' | 'Issued' | 'Active' | 'Expired' | 'Claimed' | 'Cancelled';
export type ServiceType = 'FirstFreeService' | 'SecondFreeService' | 'PeriodicMaintenance' | 'RunningRepair' | 'BodyShopAccidental' | 'BatteryHealthCheck';
export type ServiceAppointmentStatus = 'Scheduled' | 'VehicleArrived' | 'JobCardOpened' | 'InService' | 'QualityCheckPassed' | 'ReadyForDelivery' | 'Completed' | 'Cancelled';
export type NotificationChannel = 'InApp' | 'Email' | 'Sms' | 'WhatsApp';
export type PaymentPurpose = 'BookingToken' | 'DownPayment' | 'FullVehicleSettlement' | 'AccessoriesPurchase' | 'InsurancePremium' | 'ExtendedWarranty' | 'ServiceInvoice';
export type PaymentStatus = 'Initiated' | 'Processing' | 'Success' | 'Failed' | 'Refunded';
export type DocumentType = 'AadhaarCard' | 'PanCard' | 'DrivingLicense' | 'Passport' | 'ElectricityBill' | 'BankStatement' | 'SalarySlip' | 'Form16';
export type DocumentVerificationStatus = 'Pending' | 'Approved' | 'Rejected';
export type WarrantyPackageType = 'ExtendedWarranty' | 'AnnualMaintenanceContract' | 'RoadsideAssistance';
export type WarrantyStatus = 'Active' | 'Expired' | 'Terminated';
export type LeadPriority = 'Hot' | 'Warm' | 'Cold';
export type LeadStage = 'New' | 'Contacted' | 'TestDriveScheduled' | 'TestDriveDone' | 'QuotationShared' | 'Negotiation' | 'Won' | 'Lost';
export type HomeChargerSurveyStatus = 'PendingSurvey' | 'SurveyApproved' | 'SurveyRejected' | 'InstallationScheduled' | 'Installed' | 'Commissioned';
export type VehicleStockStatus = 'InTransit' | 'InStock' | 'Reserved' | 'Allocated' | 'Delivered' | 'InPdi';
export type TestDriveStatus = 'Requested' | 'Confirmed' | 'InProgress' | 'Completed' | 'Cancelled';
export type OrderStatus = 'Draft' | 'BookingConfirmed' | 'FinancingApproved' | 'Allotted' | 'InvoicingComplete' | 'ReadyForDelivery' | 'Delivered' | 'Cancelled';

// 1. Trade-In / Exchange
export interface VehicleTradeInDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  make: string;
  model: string;
  year: number;
  kilometersDriven: number;
  fuelType: string;
  condition: string;
  hasAccidentHistory: boolean;
  registrationNumber?: string;
  estimatedValue: number;
  offeredValue?: number;
  inspectionDate?: string;
  inspectorNotes?: string;
  status: TradeInStatus;
  createdAt: string;
}

export interface ValuateTradeInRequest {
  companyId: UUID;
  branchId: UUID;
  make: string;
  model: string;
  year: number;
  kilometersDriven: number;
  fuelType: string;
  condition: string;
  hasAccidentHistory: boolean;
  registrationNumber?: string;
}

// 2. Accessories
export interface VehicleAccessoryDto {
  id: UUID;
  companyId: UUID;
  name: string;
  partNumber: string;
  category: AccessoryCategory;
  compatibleVariantId?: UUID;
  price: number;
  installationCost: number;
  warrantyMonths: number;
  isActive: boolean;
}

export interface CreateAccessoryRequest {
  companyId: UUID;
  name: string;
  partNumber: string;
  category: AccessoryCategory;
  price: number;
  installationCost?: number;
  warrantyMonths?: number;
  compatibleVariantId?: UUID;
}

// 3. Insurance
export interface InsurancePolicyDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  orderId: UUID;
  buyerId: UUID;
  insurerName: string;
  policyNumber: string;
  planType: string;
  premiumAmount: number;
  idvAmount: number;
  coverageStartDate: string;
  coverageEndDate: string;
  policyDocumentUrl?: string;
  status: InsurancePolicyStatus;
  createdAt: string;
}

export interface InsurancePlanDto {
  insurerName: string;
  planName: string;
  annualPremium: number;
  cashlessGaragesCount: number;
  zeroDepIncluded: boolean;
  engineProtectionIncluded: boolean;
  roadsideAssistanceIncluded: boolean;
}

// 4. Service & Workshop
export interface ServiceAppointmentDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  vinNumber: string;
  registrationNumber: string;
  vehicleVariantId?: UUID;
  serviceType: ServiceType;
  appointmentDate: string;
  timeSlot: string;
  customerComments?: string;
  estimatedCost: number;
  actualCost?: number;
  workshopNotes?: string;
  status: ServiceAppointmentStatus;
  completedAt?: string;
  createdAt: string;
}

export interface BookServiceAppointmentRequest {
  companyId: UUID;
  branchId: UUID;
  vinNumber: string;
  registrationNumber: string;
  serviceType: ServiceType;
  appointmentDate: string;
  timeSlot: string;
  estimatedCost?: number;
  customerComments?: string;
}

// 5. Notifications
export interface CustomerNotificationDto {
  id: UUID;
  companyId: UUID;
  userId: UUID;
  title: string;
  message: string;
  channel: NotificationChannel;
  isRead: boolean;
  readAt?: string;
  actionUrl?: string;
  createdAt: string;
}

// 6. Reviews & CSAT
export interface DealershipReviewDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  orderId?: UUID;
  rating: number;
  title: string;
  reviewText: string;
  isVerifiedBuyer: boolean;
  dealerResponse?: string;
  respondedAt?: string;
  createdAt: string;
}

// 7. Analytics
export interface SalesFunnelDto {
  totalQuotations: number;
  totalOrders: number;
  pendingLoans: number;
  approvedLoans: number;
  completedDeliveries: number;
  leadToOrderConversionPct: number;
  orderToDeliveryConversionPct: number;
}

export interface StockAgingDto {
  totalVehiclesInStock: number;
  under30Days: number;
  between31And60Days: number;
  between61And90Days: number;
  over90Days: number;
  totalYardInventoryValue: number;
}

export interface RevenueAnalyticsDto {
  totalOrderValue: number;
  totalBookingAmountCollected: number;
  totalDownPaymentCollected: number;
  totalLoanDisbursed: number;
  totalAccessoriesRevenue: number;
  totalServiceRevenue: number;
}

// 8. Payments
export interface PaymentTransactionDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  orderId?: UUID;
  quotationId?: UUID;
  transactionReference: string;
  gatewayProvider: string;
  gatewayPaymentId?: string;
  gatewayOrderId?: string;
  amount: number;
  currency: string;
  purpose: PaymentPurpose;
  status: PaymentStatus;
  paymentMode: string;
  paidAt?: string;
  failureReason?: string;
  receiptUrl?: string;
  createdAt: string;
}

export interface InitiatePaymentRequest {
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  amount: number;
  purpose: PaymentPurpose;
  gatewayProvider?: string;
  paymentMode?: string;
  orderId?: UUID;
}

// 9. KYC Documents
export interface CustomerDocumentDto {
  id: UUID;
  companyId: UUID;
  userId: UUID;
  orderId?: UUID;
  documentType: DocumentType;
  documentNumber: string;
  fileUrl: string;
  fileName: string;
  fileSizeBytes: number;
  verificationStatus: DocumentVerificationStatus;
  verifiedByStaffId?: UUID;
  verifiedAt?: string;
  rejectionReason?: string;
  createdAt: string;
}

export interface UploadDocumentRequest {
  companyId: UUID;
  userId: UUID;
  documentType: DocumentType;
  documentNumber: string;
  fileUrl: string;
  fileName: string;
  fileSizeBytes: number;
  orderId?: UUID;
}

// 10. Warranty & AMC
export interface WarrantyPackageDto {
  id: UUID;
  companyId: UUID;
  name: string;
  packageType: WarrantyPackageType;
  durationMonths: number;
  kilometerLimit: number;
  price: number;
  description: string;
  isActive: boolean;
}

export interface VehicleWarrantySubscriptionDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  orderId?: UUID;
  warrantyPackageId: UUID;
  vinNumber: string;
  subscriptionNumber: string;
  startDate: string;
  endDate: string;
  pricePaid: number;
  status: WarrantyStatus;
  createdAt: string;
}

// 11. Sales CRM
export interface SalesLeadDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  assignedStaffId?: UUID;
  interestedModelId?: UUID;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  leadSource: string;
  priority: LeadPriority;
  stage: LeadStage;
  notes?: string;
  nextFollowUpDate?: string;
  lostReason?: string;
  createdAt: string;
}

export interface CreateLeadRequest {
  companyId: UUID;
  branchId: UUID;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  leadSource?: string;
  priority?: LeadPriority;
  notes?: string;
}

// 12. EV Ecosystem
export interface EvChargingStationDto {
  id: UUID;
  companyId: UUID;
  branchId?: UUID;
  stationName: string;
  locationAddress: string;
  latitude: number;
  longitude: number;
  connectorType: string;
  powerKw: number;
  tariffPerKwh: number;
  isAvailable: boolean;
}

export interface HomeChargerInstallationDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  orderId?: UUID;
  installationAddress: string;
  preferredSurveyDate: string;
  chargerModel: string;
  surveyStatus: HomeChargerSurveyStatus;
  technicianNotes?: string;
  installedAt?: string;
  createdAt: string;
}

// 13. Catalog
export interface BrandDto {
  id: UUID;
  name: string;
  code: string;
  countryOfOrigin?: string;
  logoUrl?: string;
}

export interface VehicleModelDto {
  id: UUID;
  brandId: UUID;
  name: string;
  bodyType: string;
  fuelTypes: string[];
  startingPrice: number;
  imageUrl?: string;
}

export interface VehicleVariantDto {
  id: UUID;
  modelId: UUID;
  name: string;
  transmission: string;
  engineCapacityCc: number;
  fuelType: string;
  exShowroomPrice: number;
  colorOptions: string[];
}

// 14. Inventory
export interface VehicleStockDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  variantId: UUID;
  vinNumber: string;
  engineNumber: string;
  color: string;
  status: VehicleStockStatus;
  yardLocation: string;
  receivedDate: string;
  variantName?: string;
  modelName?: string;
}

// 15. Sales Flow (Quotes, Test Drives, Bookings, Orders)
export interface QuotationDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  variantId: UUID;
  exShowroomPrice: number;
  rtoRoadTax: number;
  insuranceAmount: number;
  accessoriesTotal: number;
  discountAmount: number;
  totalOnRoadPrice: number;
  status: string;
  createdAt: string;
}

export interface TestDriveBookingDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  variantId: UUID;
  preferredDate: string;
  timeSlot: string;
  status: TestDriveStatus;
  drivingLicenseNumber?: string;
  feedbackNotes?: string;
  createdAt: string;
}

export interface VehicleBookingDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  variantId: UUID;
  bookingAmount: number;
  bookingReference: string;
  status: string;
  allocatedVin?: string;
  createdAt: string;
}

export interface VehicleOrderDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  variantId: UUID;
  bookingId?: UUID;
  orderNumber: string;
  totalAmount: number;
  downPaymentPaid: number;
  balanceDue: number;
  status: OrderStatus;
  allocatedVin?: string;
  createdAt: string;
}

export interface LoanApplicationDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  buyerId: UUID;
  orderId: UUID;
  bankName: string;
  appliedAmount: number;
  approvedAmount?: number;
  interestRatePct: number;
  tenureMonths: number;
  monthlyEmi?: number;
  status: string;
  createdAt: string;
}

export interface DeliveryInspectionDto {
  id: UUID;
  companyId: UUID;
  branchId: UUID;
  orderId: UUID;
  vinNumber: string;
  gatePassNumber: string;
  isPdiPassed: boolean;
  isDocumentKitHandedOver: boolean;
  isKeyHandedOver: boolean;
  scheduledDeliveryDate: string;
  completedDeliveryDate?: string;
  status: string;
}

export interface RtoTaxSlabDto {
  id: UUID;
  stateCode: string;
  fuelType: string;
  minPrice: number;
  maxPrice: number;
  taxPercentage: number;
}

// 16. Organization & Master Masters
export interface CompanyDto {
  id: UUID;
  name: string;
  code: string;
  contactEmail: string;
  contactPhone: string;
  taxNumberGst?: string;
  isActive: boolean;
}

export interface BranchDto {
  id: UUID;
  companyId: UUID;
  name: string;
  branchCode: string;
  address: string;
  contactPhone?: string;
  contactEmail?: string;
  isActive: boolean;
  isMainBranch: boolean;
}

export interface CountryDto {
  id: UUID;
  name: string;
  isoCode: string;
  phoneCode: string;
}

export interface StateRegionDto {
  id: UUID;
  countryId: UUID;
  name: string;
  code: string;
}

export interface CityDto {
  id: UUID;
  stateRegionId: UUID;
  name: string;
}
