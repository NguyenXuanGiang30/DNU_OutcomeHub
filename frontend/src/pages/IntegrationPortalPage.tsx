import React, { useState } from 'react';
import {
  Zap,
  RefreshCw,
  AlertTriangle,
  Send,
  Key,
  Shield,
  CheckCircle,
  Database,
  Cloud,
} from 'lucide-react';

export const IntegrationPortalPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'batches' | 'quarantine' | 'webhooks' | 'service_accounts'>('batches');
  const [testWebhookStatus, setTestWebhookStatus] = useState<string | null>(null);

  const handleTestWebhook = () => {
    setTestWebhookStatus('Đang gửi...');
    setTimeout(() => {
      setTestWebhookStatus('Gửi thành công! Mã phản hồi: 200 OK (Chữ ký HMAC-SHA256 khớp)');
    }, 800);
  };

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Cổng Tích Hợp Đồng Bộ Ngoại Vi & Webhooks (Mục 8.10)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Đồng bộ gia tăng dữ liệu SIS/LMS, cách ly và xử lý bản ghi lỗi (Quarantine), quản lý Webhooks và Service Accounts.
          </p>
        </div>

        <button className="btn btn-primary">
          <RefreshCw size={16} />
          <span>Kích Hoạt Đồng Bộ SIS Tức Thời</span>
        </button>
      </div>

      {/* Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem' }}>
        {[
          { key: 'batches', label: 'Gói Đồng Bộ Ingestion (SIS / LMS)', icon: Database },
          { key: 'quarantine', label: 'Cách Ly Lỗi Dữ Liệu (Staging Quarantine)', icon: AlertTriangle, badge: '2 Lỗi' },
          { key: 'webhooks', label: 'Sự Kiện Webhook & Bắn Thử Nghiệm', icon: Zap },
          { key: 'service_accounts', label: 'Tài Khoản Dịch Vụ (Service Accounts)', icon: Key },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key as any)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
            {tab.badge && <span className="badge badge-danger">{tab.badge}</span>}
          </button>
        ))}
      </div>

      {/* TAB 1: BATCHES */}
      {activeTab === 'batches' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Lịch Sử Các Gói Đồng Bộ Dữ Liệu Gia Tăng (Incremental Sync)</h3>
              <p className="glass-card-subtitle">Cập nhật qua con trỏ updated_since, Idempotency Key chống trùng và mã băm SHA-256</p>
            </div>
            <span className="badge badge-success">Tỷ lệ thành công: 99.88%</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Batch</th>
                  <th>Hệ Thống Nguồn</th>
                  <th>Loại Thực Thể</th>
                  <th>Số Bản Ghi</th>
                  <th>Hợp Lệ</th>
                  <th>Cách Ly</th>
                  <th>Mã Checksum SHA-256</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { id: 'BATCH-2023-088', sys: 'SIS (Quản lý đào tạo)', type: 'STUDENTS', total: 1250, valid: 1249, quar: 1, hash: '3a4b5c...89ef', status: 'HOÀN THÀNH (CÓ LỖI)' },
                  { id: 'BATCH-2023-087', sys: 'LMS Canvas', type: 'ENROLLMENTS_GRADES', total: 3420, valid: 3420, quar: 0, hash: '9f8e7d...12ab', status: 'HOÀN THÀNH' },
                  { id: 'BATCH-2023-086', sys: 'SIS (Quản lý đào tạo)', type: 'COURSES_OFFERINGS', total: 145, valid: 145, quar: 0, hash: '5c6d7e...44ff', status: 'HOÀN THÀNH' },
                ].map((b, i) => (
                  <tr key={i}>
                    <td><code>{b.id}</code></td>
                    <td><strong>{b.sys}</strong></td>
                    <td><span className="badge badge-primary">{b.type}</span></td>
                    <td>{b.total}</td>
                    <td style={{ color: 'var(--emerald-400)', fontWeight: 700 }}>{b.valid}</td>
                    <td style={{ color: b.quar > 0 ? 'var(--rose-400)' : 'var(--text-muted)', fontWeight: 700 }}>{b.quar}</td>
                    <td><code style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{b.hash}</code></td>
                    <td>
                      <span className={`badge ${b.quar > 0 ? 'badge-warning' : 'badge-success'}`}>{b.status}</span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: QUARANTINE RESOLVER */}
      {activeTab === 'quarantine' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Hàng Đợi Bản Ghi Cách Ly Lỗi (Staging Quarantine - FR-INT-04)</h3>
              <p className="glass-card-subtitle">Cách ly bản ghi không hợp lệ để tránh ô nhiễm dữ liệu học thuật; cho phép sửa chữa hoặc hủy</p>
            </div>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[
              { id: 'QUAR-901', entity: 'STUDENT (Mã SV: SV20239999)', reason: 'Mã ngành IT_NOT_FOUND không tồn tại trong danh mục CTĐT', raw: '{"studentCode":"SV20239999","programCode":"IT_NOT_FOUND"}' },
              { id: 'QUAR-902', entity: 'SCORE (Mã SV: SV20230001)', reason: 'Điểm số 11.5 vượt quá thang điểm tối đa 10.0 của bài đánh giá', raw: '{"studentCode":"SV20230001","score":11.5,"maxScore":10.0}' },
            ].map((q, idx) => (
              <div key={idx} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--rose-500)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <strong style={{ color: 'var(--rose-400)' }}>{q.id}: {q.entity}</strong>
                  <span className="badge badge-danger">Chờ Xử Lý</span>
                </div>
                <p style={{ fontSize: '0.8125rem', color: 'var(--text-primary)', marginBottom: '0.5rem' }}>• Lý do: {q.reason}</p>
                <pre style={{ backgroundColor: 'var(--bg-surface)', padding: '0.5rem', borderRadius: 'var(--radius-sm)', fontSize: '0.75rem', color: 'var(--text-secondary)', marginBottom: '0.75rem' }}>
                  {q.raw}
                </pre>
                <div style={{ display: 'flex', gap: '0.5rem' }}>
                  <button className="btn btn-sm btn-primary">Sửa & Nạp Lại (Retry)</button>
                  <button className="btn btn-sm btn-secondary">Bỏ Qua (Discard)</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 3: WEBHOOKS */}
      {activeTab === 'webhooks' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Cấu Hình Webhooks & Sự Kiện Bất Đồng Bộ (FR-INT-07)</h3>
              <p className="glass-card-subtitle">Bắn thông báo thời gian thực khi Chốt điểm, Tính xong CĐR, hoặc Kế hoạch CQI quá hạn</p>
            </div>
            <button onClick={handleTestWebhook} className="btn btn-sm btn-primary">
              <Send size={14} />
              <span>Bắn Thử Nghiệm Webhook</span>
            </button>
          </div>

          {testWebhookStatus && (
            <div style={{ padding: '0.75rem 1rem', backgroundColor: 'rgba(16, 185, 129, 0.15)', border: '1px solid var(--emerald-500)', borderRadius: 'var(--radius-md)', color: 'var(--emerald-400)', fontSize: '0.85rem', marginBottom: '1rem' }}>
              {testWebhookStatus}
            </div>
          )}

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Tên Subscription</th>
                  <th>Target URL Endpoint</th>
                  <th>Sự Kiện Đăng Ký</th>
                  <th>Xác Thực Bảo Mật</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>SIS Grade Finalize Webhook</strong></td>
                  <td><code>https://sis.daihocdanang.edu.vn/api/webhooks/outcomes</code></td>
                  <td><span className="badge badge-primary">GRADE_FINALIZED</span></td>
                  <td>HMAC-SHA256 Secret</td>
                  <td><span className="badge badge-success">HOẠT ĐỘNG</span></td>
                </tr>
                <tr>
                  <td><strong>QA Portal CQI Monitor</strong></td>
                  <td><code>https://qa.daihocdanang.edu.vn/api/webhooks/cqi</code></td>
                  <td><span className="badge badge-warning">CQI_OVERDUE</span></td>
                  <td>HMAC-SHA256 Secret</td>
                  <td><span className="badge badge-success">HOẠT ĐỘNG</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 4: SERVICE ACCOUNTS */}
      {activeTab === 'service_accounts' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Quản Lý Tài Khoản Dịch Vụ API (Service Accounts - FR-INT-08)</h3>
              <p className="glass-card-subtitle">Phân quyền theo Scope, Rate Limit và Xoay vòng khóa API Key định kỳ</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Cấp Service Account</button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1rem' }}>
            {[
              { id: 'sa_sis_sync', name: 'SIS Ingestion Worker', scope: 'read:academic, write:integration', rate: '500 req/min' },
              { id: 'sa_lms_canvas', name: 'Canvas LMS Connector', scope: 'read:course, write:measurement', rate: '1000 req/min' },
            ].map((sa, i) => (
              <div key={i} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <strong style={{ color: 'var(--primary-400)' }}>{sa.id}</strong>
                  <span className="badge badge-success">ACTIVE</span>
                </div>
                <div style={{ fontSize: '0.85rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '0.25rem' }}>{sa.name}</div>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginBottom: '0.75rem' }}>
                  • Scope: <code>{sa.scope}</code><br />
                  • Rate Limit: {sa.rate}
                </div>
                <button className="btn btn-sm btn-secondary" style={{ width: '100%' }}>Xoay Vòng Khóa (Rotate API Key)</button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
