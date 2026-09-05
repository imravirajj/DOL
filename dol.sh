#!/bin/bash

# ============================================================
#  DOL Platform — Run Script
#  Usage:
#    ./dol.sh              → Run all services (Identity API + Gateway)
#    ./dol.sh identity     → Run Identity API only
#    ./dol.sh gateway      → Run Gateway only
#    ./dol.sh build        → Build all projects without running
#    ./dol.sh stop         → Stop all running DOL services
# ============================================================

set -e

# ── Colors ───────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m' # No Color

# ── Paths ────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
IDENTITY_PROJECT="$SCRIPT_DIR/src/Services/Identity/DOL.Identity.API/DOL.Identity.API.csproj"
GATEWAY_PROJECT="$SCRIPT_DIR/src/DOL.Gateway/DOL.Gateway.csproj"
PID_DIR="$SCRIPT_DIR/.dol"

# ── URLs ─────────────────────────────────────────────────────
IDENTITY_URL="http://localhost:5065"
GATEWAY_URL="http://localhost:5104"
SWAGGER_URL="$IDENTITY_URL/swagger"

# ── Helpers ──────────────────────────────────────────────────
banner() {
    echo ""
    echo -e "${CYAN}${BOLD}"
    echo "  ╔══════════════════════════════════════════╗"
    echo "  ║         🚀  DOL Platform Runner          ║"
    echo "  ╚══════════════════════════════════════════╝"
    echo -e "${NC}"
}

log_info()    { echo -e "  ${BLUE}[INFO]${NC}    $1"; }
log_success() { echo -e "  ${GREEN}[OK]${NC}      $1"; }
log_warn()    { echo -e "  ${YELLOW}[WARN]${NC}    $1"; }
log_error()   { echo -e "  ${RED}[ERROR]${NC}   $1"; }

ensure_pid_dir() {
    mkdir -p "$PID_DIR"
}

save_pid() {
    echo "$2" > "$PID_DIR/$1.pid"
}

get_pid() {
    local pid_file="$PID_DIR/$1.pid"
    if [ -f "$pid_file" ]; then
        cat "$pid_file"
    fi
}

is_running() {
    local pid=$(get_pid "$1")
    if [ -n "$pid" ] && kill -0 "$pid" 2>/dev/null; then
        return 0
    fi
    return 1
}

cleanup() {
    echo ""
    log_warn "Shutting down DOL services..."
    stop_services
    echo ""
    log_success "All services stopped. Goodbye! 👋"
    exit 0
}

# ── Build ────────────────────────────────────────────────────
build_all() {
    log_info "Building DOL solution..."
    dotnet build "$SCRIPT_DIR/DOL.slnx" --verbosity quiet
    log_success "Build completed!"
}

# ── Start Services ───────────────────────────────────────────
start_identity() {
    if is_running "identity"; then
        log_warn "Identity API is already running (PID: $(get_pid identity))"
        return
    fi

    log_info "Starting Identity API on ${BOLD}$IDENTITY_URL${NC}"
    ASPNETCORE_ENVIRONMENT=Development dotnet run --project "$IDENTITY_PROJECT" --no-launch-profile \
        --urls "$IDENTITY_URL" \
        > "$PID_DIR/identity.log" 2>&1 &
    save_pid "identity" $!
    log_success "Identity API started (PID: $!)"
}

start_gateway() {
    if is_running "gateway"; then
        log_warn "Gateway is already running (PID: $(get_pid gateway))"
        return
    fi

    log_info "Starting Gateway on ${BOLD}$GATEWAY_URL${NC}"
    dotnet run --project "$GATEWAY_PROJECT" --no-launch-profile \
        --urls "$GATEWAY_URL" \
        > "$PID_DIR/gateway.log" 2>&1 &
    save_pid "gateway" $!
    log_success "Gateway started (PID: $!)"
}

# ── Stop Services ────────────────────────────────────────────
stop_service() {
    local name=$1
    local pid=$(get_pid "$name")
    if [ -n "$pid" ] && kill -0 "$pid" 2>/dev/null; then
        kill "$pid" 2>/dev/null
        wait "$pid" 2>/dev/null || true
        rm -f "$PID_DIR/$name.pid"
        log_success "Stopped $name (PID: $pid)"
    else
        rm -f "$PID_DIR/$name.pid"
    fi
}

stop_services() {
    stop_service "identity"
    stop_service "gateway"
}

# ── Wait for Service ────────────────────────────────────────
wait_for_service() {
    local name=$1
    local url=$2
    local max_attempts=30

    for i in $(seq 1 $max_attempts); do
        if curl -s -o /dev/null -w "" "$url" 2>/dev/null; then
            return 0
        fi
        sleep 1
    done
    return 1
}

# ── Print Status ─────────────────────────────────────────────
print_status() {
    echo ""
    echo -e "  ${BOLD}────────────────────────────────────────────${NC}"
    echo -e "  ${BOLD}  Service URLs:${NC}"
    echo -e "  ${BOLD}────────────────────────────────────────────${NC}"
    echo -e "  ${GREEN}▸${NC} Identity API :  $IDENTITY_URL"
    echo -e "  ${GREEN}▸${NC} Swagger UI   :  $SWAGGER_URL"
    echo -e "  ${GREEN}▸${NC} Gateway      :  $GATEWAY_URL"
    echo -e "  ${BOLD}────────────────────────────────────────────${NC}"
    echo ""
    echo -e "  ${YELLOW}Press Ctrl+C to stop all services${NC}"
    echo ""
}

# ── Logs ─────────────────────────────────────────────────────
tail_logs() {
    tail -f "$PID_DIR"/*.log 2>/dev/null
}

# ── Main ─────────────────────────────────────────────────────
main() {
    banner
    ensure_pid_dir

    local command="${1:-all}"

    case "$command" in
        build)
            build_all
            ;;
        identity)
            trap cleanup SIGINT SIGTERM
            build_all
            start_identity
            echo ""
            log_info "Waiting for Identity API to be ready..."
            if wait_for_service "identity" "$IDENTITY_URL/health"; then
                log_success "Identity API is ready!"
                echo -e "  ${GREEN}▸${NC} Swagger UI : $SWAGGER_URL"
            else
                log_warn "Identity API may still be starting up. Check logs: $PID_DIR/identity.log"
            fi
            echo ""
            echo -e "  ${YELLOW}Press Ctrl+C to stop${NC}"
            echo ""
            tail_logs
            ;;
        gateway)
            trap cleanup SIGINT SIGTERM
            build_all
            start_gateway
            echo ""
            echo -e "  ${GREEN}▸${NC} Gateway : $GATEWAY_URL"
            echo -e "  ${YELLOW}Press Ctrl+C to stop${NC}"
            echo ""
            tail_logs
            ;;
        stop)
            stop_services
            log_success "All DOL services stopped."
            ;;
        all|"")
            trap cleanup SIGINT SIGTERM
            build_all
            echo ""

            # Start Identity first, then Gateway
            start_identity

            log_info "Waiting for Identity API to be ready..."
            if wait_for_service "identity" "$IDENTITY_URL/health"; then
                log_success "Identity API is ready! ✅"
            else
                log_warn "Identity API may still be starting. Check logs: $PID_DIR/identity.log"
            fi

            start_gateway

            log_info "Waiting for Gateway to be ready..."
            if wait_for_service "gateway" "$GATEWAY_URL"; then
                log_success "Gateway is ready! ✅"
            else
                log_warn "Gateway may still be starting. Check logs: $PID_DIR/gateway.log"
            fi

            print_status
            tail_logs
            ;;
        *)
            echo -e "  ${RED}Unknown command: $command${NC}"
            echo ""
            echo "  Usage: ./dol.sh [command]"
            echo ""
            echo "  Commands:"
            echo "    (none)      Run all services"
            echo "    identity    Run Identity API only"
            echo "    gateway     Run Gateway only"
            echo "    build       Build all projects"
            echo "    stop        Stop all running services"
            echo ""
            exit 1
            ;;
    esac
}

main "$@"
