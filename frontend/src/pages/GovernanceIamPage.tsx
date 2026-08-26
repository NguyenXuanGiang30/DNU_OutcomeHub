import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import {
  ShieldCheck,
  Lock,
  UserCheck,
  History,
  Key,
  CheckCircle,
  AlertTriangle,
  FileCode,
  Users,
  Settings,
  Zap,
  Database,
  RefreshCw,
} from 'lucide-react';

export const GovernanceIamPage: React.FC = () => {
  const location = useLocation();

  const getSubSection = () => {
    if (location.pathname.includes('/governance/users')) return 'users';
    if (location.pathname.includes('/governance/roles-scopes')) return 'roles';
    if (location.pathname.includes('/governance/sis-lms-integration')) return 'integration';
    if (location.pathname.includes('/governance/audit-logs')) return 'audit';
    if (location.pathname.includes('/governance/system-config')) return 'config';
    return 'audit';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Quản Trị Hệ Thống, Phân Quyền & Audit Trail (Mục 8.9 & 8.10)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý tài khoản người dùng, phân quyền theo Scope, tích hợp SIS/LMS, kiểm toán bất biến và cấu hình tham số hệ thống.
          </p>
        </div>

        <button className="btn btn-primary">
          <Key size={16} />
          <span>Gán Quyền Mới Theo Scope</span>
        </button>
      </div>

      {/* Navigation Sub-Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'users', label: '1. Người Dùng Hệ Thống', icon: Users },
          { key: 'roles', label: '2. Vai Trò & Phân Quyền Scope', icon: UserCheck },
          { key: 'sod', label: '3. Tách Biệt Nhiệm Vụ (SoD)', icon: Lock },
          { key: 'integration', label: '4. Tích Hợp SIS/LMS & Webhook', icon: Zap },
          { key: 'audit', label: '5. Nhật Ký Kiểm Toán (Hash Chain)', icon: History, badge: 'SHA-256' },
          { key: 'config', label: '6. Cấu Hình Hệ Thống', icon: Settings },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
            {tab.badge && <span className="badge badge-bloom badge-cyan">{tab.badge}</span>}
          </button>
        ))}
      </div>

      {/* TAB 1: USERS */}
      {activeTab === 'users' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Người Dùng & Trạng Thái Xác Thực</h3>
              <p className="glass-card-subtitle">Hỗ trợ SSO Microsoft 365 và Tài khoản cục bộ</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Thêm Người Dùng</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Người Dùng</th>
                  <th>Họ Và Tên</th>
                  <th>Email DNU</th>
                  <th>Vai Trò Mặc Định</th>
                  <th>Đơn Vị Công Tác</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'USR-001', name: 'TS. Lê Hải Nam', email: 'nam.lh@dnu.edu.vn', role: 'LECTURER', dept: 'Khoa CNTT', status: 'HOẠT ĐỘNG' },
                  { code: 'USR-002', name: 'PGS. TS. Trần Văn Bình', email: 'binh.tv@dnu.edu.vn', role: 'DEAN', dept: 'Khoa CNTT', status: 'HOẠT ĐỘNG' },
                  { code: 'USR-003', name: 'Admin Quản Trị Hệ Thống', email: 'admin@outcomehub.dnu.edu.vn', role: 'ADMIN', dept: 'Phòng ĐBCL & CNTT', status: 'HOẠT ĐỘNG' },
                ].map((u) => (
                  <tr key={u.code}>
                    <td><code>{u.code}</code></td>
                    <td style={{ fontWeight: 700 }}>{u.name}</td>
                    <td>{u.email}</td>
                    <td><span className="badge badge-primary">{u.role}</span></td>
                    <td>{u.dept}</td>
                    <td><span className="badge badge-success">{u.status}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: ROLES & SCOPES */}
      {activeTab === 'roles' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Phân Quyền Ma Trận Vai Trò Theo Scope (Khoa - CTĐT - Lớp - Đợt)</h3>
              <p className="glass-card-subtitle">Server-side RLS enforcement đảm bảo không rò rỉ dữ liệu ngoài thẩm quyền</p>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Tên Vai Trò</th>
                  <th>Phạm Vi Scope</th>
                  <th>Quyền Đọc (SELECT)</th>
                  <th>Quyền Ghi (INSERT/UPDATE)</th>
                  <th>Quyền Phê Duyệt</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>ADMIN (Quản trị hệ thống)</strong></td>
                  <td><span className="badge badge-primary">Toàn Trường</span></td>
                  <td>Toàn bộ hệ thống</td>
                  <td>Cấu hình, Người dùng, System Params</td>
                  <td>Phân quyền & Khóa hệ thống</td>
                </tr>
                <tr>
                  <td><strong>DEAN (Trưởng Khoa / Viện)</strong></td>
                  <td><span className="badge badge-cyan">Khoa CNTT</span></td>
                  <td>Dữ liệu trong Khoa</td>
                  <td>Phê duyệt CTĐT, Đề cương, CQI</td>
                  <td>Công bố kết quả CĐR Khoa</td>
                </tr>
                <tr>
                  <td><strong>LECTURER (Giảng viên)</strong></td>
                  <td><span className="badge badge-secondary">Lớp Được Phân Công</span></td>
                  <td>Lớp giảng dạy & Môn phụ trách</td>
                  <td>Nhập điểm Rubric, Soạn ĐCCT</td>
                  <td>Ký xác nhận điểm lớp</td>
                </tr>
                <tr>
                  <td><strong>STUDENT (Sinh viên)</strong></td>
                  <td><span className="badge badge-secondary">Cá Nhân</span></td>
                  <td>Điểm CĐR cá nhân & CTĐT đang học</td>
                  <td>Không có quyền ghi</td>
                  <td>Không có</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: SEPARATION OF DUTIES (SoD) */}
      {activeTab === 'sod' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Chính Sách Tách Biệt Nhiệm Vụ (Separation of Duties - SoD)</h3>
              <p className="glass-card-subtitle">Ngăn chặn xung đột lợi ích: Người nhập điểm không được tự phê duyệt/công bố kết quả</p>
            </div>
            <span className="badge badge-success">3 Quy Tắc SoD Kích Hoạt</span>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1rem' }}>
            {[
              { rule: 'SoD-01: Nhập Điểm vs Phê Duyệt', desc: 'Giảng viên chấm thi không được kiêm nhiệm quyền Phê duyệt kết quả đợt đo', status: 'TUÂN THỦ' },
              { rule: 'SoD-02: Soạn Thảo vs Thẩm Định ĐCCT', desc: 'Tác giả biên soạn Đề cương chi tiết không được tự ký duyệt thẩm định', status: 'TUÂN THỦ' },
              { rule: 'SoD-03: Quản Trị Hệ Thống vs Nhập Điểm', desc: 'Tài khoản Quản trị viên (Admin) không được can thiệp sửa điểm trực tiếp', status: 'TUÂN THỦ' },
            ].map((r, idx) => (
              <div key={idx} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <strong style={{ color: 'var(--primary-400)' }}>{r.rule}</strong>
                  <span className="badge badge-success">{r.status}</span>
                </div>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>{r.desc}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 4: INTEGRATION SIS/LMS */}
      {activeTab === 'integration' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Tích Hợp Đồng Bộ Ngoại Vi SIS / LMS & Webhooks</h3>
              <p className="glass-card-subtitle">Đồng bộ gia tăng và quản lý cổng gửi Webhook bảo mật</p>
            </div>
            <button className="btn btn-sm btn-primary">Đồng Bộ Tức Thời SIS</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Hệ Thống Nguồn</th>
                  <th>Loại Tích Hợp</th>
                  <th>Giao Thức</th>
                  <th>Tần Suất Đồng Bộ</th>
                  <th>Trạng Thái Kết Nối</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>SIS Quản Lý Đào Tạo</strong></td>
                  <td>Sinh viên, Học phần, Lớp tín chỉ</td>
                  <td>REST API / OAuth2</td>
                  <td>Hàng ngày (02:00 AM)</td>
                  <td><span className="badge badge-success">KẾT NỐI ỔN ĐỊNH</span></td>
                </tr>
                <tr>
                  <td><strong>LMS Canvas</strong></td>
                  <td>Bảng điểm quá trình, Quiz</td>
                  <td>LTI 1.3 / REST API</td>
                  <td>Mỗi 6 tiếng</td>
                  <td><span className="badge badge-success">KẾT NỐI ỔN ĐỊNH</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 5: AUDIT LOGS */}
      {activeTab === 'audit' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <History size={20} className="text-emerald-400" />
                Chuỗi Nhật Ký Kiểm Toán Bất Biến (Immutable Hash Chain - FR-ADM-05)
              </h3>
              <p className="glass-card-subtitle">
                Mỗi hành động thay đổi cấu hình, nhập/xuất điểm hoặc phê duyệt đều được gắn mã băm liên kết không thể sửa xóa
              </p>
            </div>
            <span className="badge badge-success">Toàn Vẹn 100% (Không Vi Phạm)</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Thời Điểm (UTC)</th>
                  <th>Người Thực Hiện</th>
                  <th>Hành Động Nghiệp Vụ</th>
                  <th>Đối Tượng Tác Động</th>
                  <th>Scope Dữ Liệu</th>
                  <th>Mã Băm Bất Biến (Current Hash)</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { time: '26/08/2026 13:30:15', actor: 'TS. Lê Hải Nam (GV001)', action: 'CHẤM ĐIỂM RUBRIC A2', target: 'Lớp 17IT01 - Môn Lập trình .NET', scope: 'Khoa CNTT', hash: 'e4f5a6b7...c8d9' },
                  { time: '26/08/2026 11:15:40', actor: 'Admin Hệ Thống', action: 'XOAY VÒNG KHÓA API KEY', target: 'Service Account sa_sis_sync', scope: 'Toàn Trường', hash: '7a8b9c0d...1e2f' },
                  { time: '25/08/2026 16:45:22', actor: 'PGS. TS. Trần Văn Bình', action: 'PHÊ DUYỆT ĐỀ CƯƠNG BM13', target: 'Học phần IT4101 v2.0', scope: 'Ngành KTPM', hash: '3d4e5f6a...7b8c' },
                ].map((log, i) => (
                  <tr key={i}>
                    <td><code style={{ fontSize: '0.75rem' }}>{log.time}</code></td>
                    <td><strong>{log.actor}</strong></td>
                    <td><span className="badge badge-primary">{log.action}</span></td>
                    <td>{log.target}</td>
                    <td><span className="badge badge-secondary">{log.scope}</span></td>
                    <td><code style={{ fontSize: '0.75rem', color: 'var(--emerald-400)' }}>{log.hash}</code></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 6: CONFIG */}
      {activeTab === 'config' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Cấu Hình Tham Số Hệ Thống OBE Toàn Trường</h3>
              <p className="glass-card-subtitle">Thiết lập ngưỡng mặc định và tham số thuật toán</p>
            </div>
            <button className="btn btn-sm btn-primary">Lưu Cấu Hình</button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '1.25rem' }}>
            <div className="form-group">
              <label className="form-label">Ngưỡng Đạt Chuẩn Cá Nhân Mặc Định (θ_ind)</label>
              <input type="number" step="0.5" defaultValue="6.0" className="form-input" />
            </div>
            <div className="form-group">
              <label className="form-label">Ngưỡng Đạt Chuẩn Của Khóa Mặc Định (θ_coh)</label>
              <input type="number" step="1" defaultValue="80" className="form-input" />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
