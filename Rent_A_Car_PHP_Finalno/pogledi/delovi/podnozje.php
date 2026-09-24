<footer class="podnozje"><span>Rent-a-Car sistem</span><span>© <?=date('Y')?> · PHP MVC + REST</span></footer>
<script>
document.querySelectorAll('.zatvori-obavestenje').forEach(function(btn){btn.addEventListener('click',function(){btn.closest('[data-obavestenje]').remove();});});
setTimeout(function(){document.querySelectorAll('[data-obavestenje]').forEach(function(x){x.classList.add('sakrij');});},4500);
</script>
</body></html>
